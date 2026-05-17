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
public class BidService(
    IBidRepository repository,
    IAuctionLotRepository auctionLots,
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
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            throw new TmsException("Cannot delete: bid not found", HttpStatusCode.NotFound);
        }

        await repository.DeleteAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Guid> UpsertAsync(Guid driverId, BidCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var auctionInDb = await auctionLots.GetByIdAsync(dto.AuctionLotId, cancellationToken);
        if (auctionInDb == null)
        {
            throw new TmsException("Auction Lot not found", HttpStatusCode.NotFound);
        }

        if (auctionInDb.Status == AuctionStatus.Finished || auctionInDb.EndsAt < DateTime.UtcNow)
        {
            throw new TmsException("Cannot place bid: auction is not active", HttpStatusCode.BadRequest);
        }

        var existingBid = await repository.GetByDriverAndLotAsync(driverId, dto.AuctionLotId, cancellationToken);

        if (existingBid != null)
        {
            existingBid.Note = dto.Note;
            existingBid.Rate = dto.Rate;
            existingBid.CreatedAt = DateTime.UtcNow;

            await repository.UpdateAsync(existingBid, cancellationToken);
            return existingBid.Id;
        }

        try
        {
            var newBid = mapper.Map<Bid>(dto);
            newBid.DriverCreatedId = driverId;
            newBid.CreatedAt = DateTime.UtcNow;

            await repository.AddAsync(newBid, cancellationToken);
            return newBid.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create bid. Ensure data is valid.", HttpStatusCode.BadRequest);
        }
    }
}