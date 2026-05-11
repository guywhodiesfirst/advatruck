namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class DispatcherService(
    IDispatcherRepository repository,
    IMapper mapper) : IDispatcherService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<DispatcherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<DispatcherDto>>(entities);
    }

    /// <inheritdoc/>
    public async Task<DispatcherDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        return entity == null ?
            throw new TmsException($"Dispatcher with ID {id} not found", HttpStatusCode.NotFound) : mapper.Map<DispatcherDto>(entity);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(DispatcherCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = mapper.Map<Dispatcher>(dto);
            await repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create dispatcher. Ensure data is valid.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc/>
    public async Task<DispatcherDto> UpdateAsync(DispatcherCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (!dto.Id.HasValue)
        {
            throw new TmsException("Dispatcher ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingEntity = await repository.GetByIdAsync(dto.Id.Value, cancellationToken);
        if (existingEntity == null)
        {
            throw new TmsException("Cannot update: dispatcher not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingEntity);
        await repository.UpdateAsync(existingEntity, cancellationToken);

        return mapper.Map<DispatcherDto>(existingEntity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException("Cannot delete: dispatcher not found", HttpStatusCode.NotFound);
        }

        await repository.DeleteAsync(entity, cancellationToken);
    }
}