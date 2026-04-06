using Core.Entities;
using Business.Interfaces;
using Data.Interfaces;

namespace Business.Services;

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
        return await driverRepository.GetByIdAsync(id, cancellationToken);
    }
    
    /// <inheritdoc/>
    public async Task<Driver?> LoginAsync(string email, CancellationToken cancellationToken = default)
    {
        return await driverRepository.GetByEmailAsync(email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        driver.RegistrationDate = DateTime.UtcNow;

        return await driverRepository.AddAsync(driver, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var driver = await driverRepository.GetByIdAsync(id, cancellationToken);

        if (driver == null)
            return;

        await driverRepository.DeleteAsync(driver, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        return await driverRepository.UpdateAsync(driver, cancellationToken);
    }
}