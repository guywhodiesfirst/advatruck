namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc />
public class LoadService(
    ILoadRepository loadRepository,
    IMapper mapper) : ILoadService
{
    /// <inheritdoc />
    public async Task<IEnumerable<LoadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var loads = await loadRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<LoadDto>>(loads);
    }

    /// <inheritdoc />
    public async Task<LoadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var load = await loadRepository.GetByIdAsync(id, cancellationToken);

        return load == null
            ? throw new TmsException($"Load with ID {id} not found", HttpStatusCode.NotFound)
            : mapper.Map<LoadDto>(load);
    }

    // TODO: add stops timestamps validation

    /// <inheritdoc />
    public async Task<Guid> CreateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var load = mapper.Map<Load>(dto);
            load.CreatedAt = DateTime.UtcNow;

            UpdateLoadStatus(load);
            HandleClosedAt(load);

            foreach (var stop in load.LoadStops)
            {
                stop.LoadId = load.Id;
            }

            await loadRepository.AddAsync(load, cancellationToken);
            return load.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create Load. Check if Driver and Dispatcher exist.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc />
    public async Task<LoadDto> UpdateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(dto.Id, cancellationToken);

        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingLoad);
        UpdateLoadStatus(existingLoad);
        HandleClosedAt(existingLoad);

        await loadRepository.UpdateAsync(existingLoad, cancellationToken);
        return mapper.Map<LoadDto>(existingLoad);
    }

    /// <inheritdoc />
    public async Task<LoadDto> UpdateStatusAsync(LoadStatusUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(dto.LoadId, cancellationToken);

        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        existingLoad.LoadStatus = dto.LoadStatus;
        HandleClosedAt(existingLoad);

        await loadRepository.UpdateAsync(existingLoad, cancellationToken);
        return mapper.Map<LoadDto>(existingLoad);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var load = await loadRepository.GetByIdAsync(id, cancellationToken);

        if (load == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        await loadRepository.DeleteAsync(load, cancellationToken);
    }

    private static void UpdateLoadStatus(Load load)
    {
        var utcNow = DateTime.UtcNow;
        var firstStop = load.LoadStops.OrderBy(s => s.Timestamp).FirstOrDefault();

        if (firstStop != null)
        {
            load.LoadStatus = utcNow >= firstStop.Timestamp
                ? LoadStatus.Ongoing
                : LoadStatus.Scheduled;
        }
    }

    private static void HandleClosedAt(Load load)
    {
        if (load.LoadStatus is LoadStatus.Completed or LoadStatus.Cancelled)
        {
            load.ClosedAt ??= DateTime.UtcNow;
        }
        else
        {
            load.ClosedAt = null;
        }
    }
}