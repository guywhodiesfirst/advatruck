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

        if (entity == null)
        {
            throw new TmsException($"Dispatcher with ID {id} not found", HttpStatusCode.NotFound);
        }

        return mapper.Map<DispatcherDto>(entity);
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

    /// <inheritdoc/>
    public async Task<DispatcherProfileDto?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var dispatcher = await repository.GetByEmailAsync(email, cancellationToken);

        if (dispatcher == null)
        {
            throw new TmsException($"Profile for email {email} not found", HttpStatusCode.NotFound);
        }

        return MapToProfileDto(dispatcher);
    }

    /// <inheritdoc/>
    public async Task<DispatcherProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dispatcher = await repository.GetByIdAsync(id, cancellationToken);

        if (dispatcher == null)
        {
            throw new TmsException($"Dispatcher profile with ID {id} not found", HttpStatusCode.NotFound);
        }

        return MapToProfileDto(dispatcher);
    }

    private static DispatcherProfileDto MapToProfileDto(Dispatcher dispatcher)
    {
        return new DispatcherProfileDto
        {
            Id = dispatcher.Id,
            FirstName = dispatcher.User?.FirstName ?? "N/A",
            LastName = dispatcher.User?.LastName ?? "N/A",
            Email = dispatcher.User?.Email ?? "N/A",
            PhoneNumber = dispatcher.User?.PhoneNumber,
            RegistrationDate = dispatcher.User?.RegistrationDate ?? DateTime.MinValue,
            LoadCount = dispatcher.Loads?.Count ?? 0,
        };
    }
}