using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Data context for the transport management system.
/// </summary>
/// <param name="options">Data context options.</param>
public class TmsDataContext(DbContextOptions<TmsDataContext> options)
    : DbContext(options)
{
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        base.OnModelCreating(modelBuilder);
    }
}