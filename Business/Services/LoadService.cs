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
    INotificationService notificationService,
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

    /// <inheritdoc />
    public async Task<Guid> CreateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var load = mapper.Map<Load>(dto);
            load.Id = Guid.NewGuid();
            load.CreatedAt = DateTime.UtcNow;

            UpdateLoadStatus(load);
            HandleClosedAt(load);

            foreach (var stop in load.LoadStops)
            {
                stop.LoadId = load.Id;
            }

            await loadRepository.AddAsync(load, cancellationToken);

            if (load.DriverId.HasValue)
            {
                await notificationService.PublishLoadAssignedAsync(load.DriverId.Value, load.Id);
            }

            return load.Id;
        }
        catch (TmsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TmsException($"Failed to create Load: {ex.Message}", ex, HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc />
    public async Task<LoadDto> UpdateAsync(Guid id, LoadCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(id, cancellationToken);
        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        var oldDriverId = existingLoad.DriverId;
        var oldStatus = existingLoad.LoadStatus;

        mapper.Map(dto, existingLoad);

        UpdateLoadStatus(existingLoad);
        HandleClosedAt(existingLoad);

        await loadRepository.UpdateAsync(existingLoad, cancellationToken);

        if (existingLoad.DriverId.HasValue)
        {
            if (existingLoad.DriverId != oldDriverId)
            {
                await notificationService.PublishLoadAssignedAsync(existingLoad.DriverId.Value, existingLoad.Id);
            }

            if (existingLoad.LoadStatus == LoadStatus.Ongoing && oldStatus == LoadStatus.Scheduled)
            {
                await notificationService.PublishLoadStartedAsync(existingLoad.DriverId.Value, existingLoad.Id);
            }
        }

        return mapper.Map<LoadDto>(existingLoad);
    }

    /// <inheritdoc />
    public async Task<LoadDto> UpdateStatusAsync(Guid id, LoadStatusUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(id, cancellationToken);
        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        existingLoad.LoadStatus = dto.LoadStatus;
        HandleClosedAt(existingLoad);

        await loadRepository.UpdateAsync(existingLoad, cancellationToken);

        if (dto.LoadStatus == LoadStatus.Cancelled && existingLoad.DriverId.HasValue)
        {
            await notificationService.PublishLoadCanceledAsync(existingLoad.DriverId.Value, existingLoad.Id);
        }

        return mapper.Map<LoadDto>(existingLoad);
    }

    /// <inheritdoc/>
    public async Task<LoadDto> AssignDriverAsync(Guid id, LoadAssignDriverDto dto, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(id, cancellationToken);
        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        existingLoad.DriverId = dto.DriverId;
        existingLoad.DriverCharge = dto.DriverCharge;
        HandleClosedAt(existingLoad);

        await loadRepository.UpdateAsync(existingLoad, cancellationToken);

        var updatedLoad = await loadRepository.GetByIdAsync(existingLoad.Id, cancellationToken);

        await notificationService.PublishLoadAssignedAsync(dto.DriverId, existingLoad.Id);

        return mapper.Map<LoadDto>(updatedLoad);
    }

    public async Task<LoadDto> DeassignDriverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingLoad = await loadRepository.GetByIdAsync(id, cancellationToken);
        if (existingLoad == null)
        {
            throw new TmsException("Load not found", HttpStatusCode.NotFound);
        }

        var driverId = existingLoad.DriverId;
        if (driverId != null)
        {
            existingLoad.DriverId = null;
            await notificationService.PublishLoadDeassignedAsync(driverId.Value, id);
        }

        existingLoad.DriverCharge = null;

        var updatedLoad = await loadRepository.UpdateAsync(existingLoad, cancellationToken);

        return mapper.Map<LoadDto>(updatedLoad);
    }

    /// <inheritdoc/>
    public async Task<LoadDto?> GetActiveLoadByIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var load = await loadRepository.GetNextActiveLoadByDriverIdAsync(driverId, cancellationToken);
            return load == null ? null : mapper.Map<LoadDto>(load);
        }
        catch (Exception ex)
        {
            throw new TmsException($"Error retrieving active load: {ex.Message}", HttpStatusCode.InternalServerError);
        }
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