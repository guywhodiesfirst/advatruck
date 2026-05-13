namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class AdminRepository(TmsDataContext context) : IAdminRepository
{
    /// <inheritdoc />
    public async Task<Admin?> GetByIdAsync(Guid adminId, CancellationToken cancellationToken = default)
        => await context.Admins
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == adminId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Admin>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Admins
            .Include(e => e.User)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        await context.Admins.AddAsync(admin, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return admin.Id;
    }

    /// <inheritdoc />
    public async Task<Admin> UpdateAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        context.Admins.Update(admin);
        await context.SaveChangesAsync(cancellationToken);
        return admin;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        context.Admins.Remove(admin);
        await context.SaveChangesAsync(cancellationToken);
    }
}