using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class TmsDataContext(DbContextOptions<TmsDataContext> options)
    : DbContext(options)
{
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    
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
            .HasKey(dl => new { dl.DriverId, dl.LastModified });
        
        modelBuilder.Entity<DriverLocation>()
            .HasIndex(d => d.DriverId);

        base.OnModelCreating(modelBuilder);
    }
}