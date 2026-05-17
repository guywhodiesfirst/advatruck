namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc />
public class VehicleService(IVehicleRepository repository, IMapper mapper) : IVehicleService
{
    /// <inheritdoc />
    public async Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vehicles = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<VehicleDto>>(vehicles);
    }

    /// <inheritdoc />
    public async Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return mapper.Map<VehicleDto>(entity)
               ?? throw new TmsException($"Vehicle with ID {id} not found", HttpStatusCode.NotFound);
    }

    /// <inheritdoc />
    public async Task<Guid> CreateAsync(VehicleCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var vehicle = mapper.Map<Vehicle>(dto);
        vehicle.Id = Guid.NewGuid();

        try
        {
            return await repository.AddAsync(vehicle, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to create Vehicle", ex, HttpStatusCode.InternalServerError);
        }
    }

    /// <inheritdoc />
    public async Task<VehicleDto> UpdateAsync(Guid id, VehicleCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new TmsException("Vehicle ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingVehicle = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TmsException($"Vehicle with ID {id} not found", HttpStatusCode.NotFound);

        mapper.Map(dto, existingVehicle);

        try
        {
            await repository.UpdateAsync(existingVehicle, cancellationToken);
            return mapper.Map<VehicleDto>(existingVehicle);
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to update Vehicle", ex, HttpStatusCode.InternalServerError);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
                     ?? throw new TmsException($"Vehicle with ID {id} not found", HttpStatusCode.NotFound);

        try
        {
            await repository.DeleteAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to delete Vehicle", ex, HttpStatusCode.InternalServerError);
        }
    }
}