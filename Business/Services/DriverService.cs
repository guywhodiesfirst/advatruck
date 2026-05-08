namespace Business.Services;

using System.Net;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using Data.Interfaces;

/// <inheritdoc/>
public class DriverService(IDriverRepository driverRepository) : IDriverService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Driver>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await driverRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var driver = await driverRepository.GetByIdAsync(id, cancellationToken);

        return driver
               ?? throw new TmsException($"Driver with ID {id} not found", HttpStatusCode.NotFound);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        return await driverRepository.AddAsync(driver, cancellationToken);
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

    /// <inheritdoc/>
    public async Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        var exists = await driverRepository.GetByIdAsync(driver.Id, cancellationToken);
        if (exists == null)
        {
            throw new TmsException("Cannot update: driver not found", HttpStatusCode.NotFound);
        }

        return await driverRepository.UpdateAsync(driver, cancellationToken);
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
}