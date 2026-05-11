namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class DriverService(
    IDriverRepository driverRepository,
    IMapper mapper) : IDriverService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var drivers = await driverRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<DriverDto>>(drivers);
    }

    /// <inheritdoc/>
    public async Task<DriverDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var driver = await driverRepository.GetByIdAsync(id, cancellationToken);

        if (driver == null)
        {
            throw new TmsException($"Driver with ID {id} not found", HttpStatusCode.NotFound);
        }

        return mapper.Map<DriverDto>(driver);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(DriverCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var driver = mapper.Map<Driver>(dto);
            await driverRepository.AddAsync(driver, cancellationToken);
            return driver.Id;
        }
        catch (Exception)
        {
            throw new TmsException("Failed to create driver. Ensure UserId is valid.", HttpStatusCode.BadRequest);
        }
    }

    /// <inheritdoc/>
    public async Task<DriverDto> UpdateAsync(DriverCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (!dto.Id.HasValue)
        {
            throw new TmsException("Driver ID is required for update", HttpStatusCode.BadRequest);
        }

        var existingDriver = await driverRepository.GetByIdAsync(dto.Id.Value, cancellationToken);
        if (existingDriver == null)
        {
            throw new TmsException("Cannot update: driver not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existingDriver);
        await driverRepository.UpdateAsync(existingDriver, cancellationToken);

        return mapper.Map<DriverDto>(existingDriver);
    }

    /// <inheritdoc/>
    public async Task<DriverProfileDto?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var driver = await driverRepository.GetByEmailAsync(email, cancellationToken);

        if (driver == null)
        {
            throw new TmsException($"Profile for email {email} not found", HttpStatusCode.NotFound);
        }

        return new DriverProfileDto
        {
            Id = driver.Id,
            FirstName = driver.User.FirstName,
            LastName = driver.User.LastName,
            Email = driver.User.Email!,
            PhoneNumber = driver.User.PhoneNumber,
            LastLocation = driver.DriverLocations
                .FirstOrDefault()?.Location,
            RegistrationDate = driver.User.RegistrationDate,
        };
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var driver = await driverRepository.GetByIdAsync(id, cancellationToken);

        if (driver == null)
        {
            throw new TmsException("Cannot delete: driver not found", HttpStatusCode.NotFound);
        }

        await driverRepository.DeleteAsync(driver, cancellationToken);
    }
}