namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class AdminService(
    IAdminRepository repository,
    IMapper mapper) : IAdminService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<AdminDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<AdminDto>>(entities);
    }

    /// <inheritdoc/>
    public async Task<AdminDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException($"Admin with ID {id} not found", HttpStatusCode.NotFound);
        }

        return mapper.Map<AdminDto>(entity);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(AdminCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = mapper.Map<Admin>(dto);
            await repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create admin. Ensure data is valid.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc/>
    public async Task<AdminDto> UpdateAsync(AdminCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (!dto.Id.HasValue)
        {
            throw new TmsException("Admin ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingEntity = await repository.GetByIdAsync(dto.Id.Value, cancellationToken);
        if (existingEntity == null)
        {
            throw new TmsException("Cannot update: admin not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingEntity);
        await repository.UpdateAsync(existingEntity, cancellationToken);

        return mapper.Map<AdminDto>(existingEntity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException("Cannot delete: admin not found", HttpStatusCode.NotFound);
        }

        await repository.DeleteAsync(entity, cancellationToken);
    }
}