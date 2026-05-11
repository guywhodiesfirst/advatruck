namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class BidService(
    IBidRepository repository,
    IMapper mapper) : IBidService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<BidDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<BidDto>>(entities);
    }

    /// <inheritdoc/>
    public async Task<BidDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException($"Bid with ID {id} not found", HttpStatusCode.NotFound);
        }

        return mapper.Map<BidDto>(entity);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(BidCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = mapper.Map<Bid>(dto);
            await repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create bid. Ensure data is valid.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc/>
    public async Task<BidDto> UpdateAsync(BidCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
        {
            throw new TmsException("Bid ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingEntity = await repository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingEntity == null)
        {
            throw new TmsException("Cannot update: bid not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingEntity);
        await repository.UpdateAsync(existingEntity, cancellationToken);

        return mapper.Map<BidDto>(existingEntity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException("Cannot delete: bid not found", HttpStatusCode.NotFound);
        }

        await repository.DeleteAsync(entity, cancellationToken);
    }
}