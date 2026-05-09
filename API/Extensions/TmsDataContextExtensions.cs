namespace API.Extensions;

using Core.Entities;
using Core.Identity;
using Data;
using Microsoft.AspNetCore.Identity;

public static class TmsDataContextExtensions
{
    public static void SeedData(this TmsDataContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();

        var adminRoleId = Guid.NewGuid();
        var driverRoleId = Guid.NewGuid();
        var roles = new List<IdentityRole<Guid>>
        {
            new() { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new() { Id = driverRoleId, Name = "Driver", NormalizedName = "DRIVER" },
            new() { Id = Guid.NewGuid(), Name = "Dispatcher", NormalizedName = "DISPATCHER" },
        };
        context.Roles.AddRange(roles);

        var driverUser = new AppUser
        {
            Id = Guid.NewGuid(),
            FirstName = "Іван",
            LastName = "Петренко",
            Email = "driver@tms.com",
            NormalizedEmail = "DRIVER@TMS.COM",
            UserName = "driver@tms.com",
            NormalizedUserName = "DRIVER@TMS.COM",
            SecurityStamp = Guid.NewGuid().ToString(),
            RegistrationDate = DateTime.UtcNow,
        };

        driverUser.PasswordHash = hasher.HashPassword(driverUser, "Password123!");

        var driverEntity = new Driver
        {
            Id = Guid.NewGuid(),
            User = driverUser,
            Note = "Досвідчений водій",
        };

        var dispatcherUser = new AppUser
        {
            Id = Guid.NewGuid(),
            FirstName = "Олексій",
            LastName = "Коваленко",
            Email = "admin@tms.com",
            NormalizedEmail = "ADMIN@TMS.COM",
            UserName = "admin@tms.com",
            NormalizedUserName = "ADMIN@TMS.COM",
            SecurityStamp = Guid.NewGuid().ToString(),
            RegistrationDate = DateTime.UtcNow,
        };

        dispatcherUser.PasswordHash = hasher.HashPassword(dispatcherUser, "Admin123!");

        var dispatcherEntity = new Dispatcher
        {
            Id = Guid.NewGuid(),
            User = dispatcherUser,
        };

        context.Users.AddRange(driverUser, dispatcherUser);
        context.Drivers.Add(driverEntity);
        context.Dispatchers.Add(dispatcherEntity);

        context.UserRoles.AddRange(
            new IdentityUserRole<Guid> { UserId = driverUser.Id, RoleId = driverRoleId },
            new IdentityUserRole<Guid> { UserId = dispatcherUser.Id, RoleId = adminRoleId });

        context.SaveChanges();
    }
}