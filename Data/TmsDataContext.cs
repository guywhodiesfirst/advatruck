namespace Data;

using Core.Entities;
using Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Data context for the transport management system.
/// </summary>
/// <param name="options">Data context options.</param>
public class TmsDataContext(DbContextOptions<TmsDataContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Driver> Drivers { get; set; }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<DriverLocation> DriverLocations { get; set; }

    public DbSet<LoadLocation> LoadLocations { get; set; }

    public DbSet<Load> Loads { get; set; }

    public DbSet<Dispatcher> Dispatchers { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Vehicle)
            .WithOne(v => v.Driver)
            .HasForeignKey<Vehicle>(v => v.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DriverLocation>()
            .OwnsOne(d => d.Location);

        modelBuilder.Entity<DriverLocation>()
            .HasOne(dl => dl.Driver)
            .WithMany(d => d.DriverLocations)
            .HasForeignKey(dl => dl.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DriverLocation>()
            .HasKey(dl => new { dl.DriverId, LastModified = dl.UpdateTime });

        modelBuilder.Entity<DriverLocation>()
            .HasIndex(d => d.DriverId);

        modelBuilder.Entity<LoadLocation>()
            .OwnsOne(l => l.Location);

        base.OnModelCreating(modelBuilder);
    }
}