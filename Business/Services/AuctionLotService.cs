namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class AuctionLotService(
    IAuctionLotRepository repository,
    IMapper mapper) : IAuctionLotService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<AuctionLotDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<AuctionLotDto>>(entities);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AuctionLotDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllActiveAsync(cancellationToken);
        return mapper.Map<IEnumerable<AuctionLotDto>>(entities);
    }

    /// <inheritdoc/>
    public async Task<AuctionLotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException($"AuctionLot with ID {id} not found", HttpStatusCode.NotFound);
        }

        return mapper.Map<AuctionLotDto>(entity);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = mapper.Map<AuctionLot>(dto);
            entity.StartsAt = DateTime.UtcNow;

            await repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create auctionLot. Ensure data is valid.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc/>
    public async Task<AuctionLotDto> UpdateAsync(Guid id, AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new TmsException("AuctionLot ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingEntity = await repository.GetByIdAsync(id, cancellationToken);
        if (existingEntity == null)
        {
            throw new TmsException("Cannot update: auctionLot not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingEntity);
        await repository.UpdateAsync(existingEntity, cancellationToken);

        return mapper.Map<AuctionLotDto>(existingEntity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException("Cannot delete: auctionLot not found", HttpStatusCode.NotFound);
        }

        await repository.DeleteAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<AuctionLotDto> UpdateStatusAsync(Guid id, AuctionStatus status, CancellationToken cancellationToken = default)
    {
        if (status == AuctionStatus.Active)
        {
            throw new TmsException("Cannot reactivate a finished auction. Please, create a new one", HttpStatusCode.BadRequest);
        }

        var auction = await repository.GetByIdAsync(id, cancellationToken);

        if (auction == null)
        {
            throw new TmsException("Auction not found", HttpStatusCode.NotFound);
        }

        if (auction.Status != status)
        {
            auction.Status = status;
            auction.EndsAt = DateTime.UtcNow;
            await repository.UpdateAsync(auction, cancellationToken);
        }

        return mapper.Map<AuctionLotDto>(auction);
    }
}