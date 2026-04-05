using Data;
using Data.Models;

namespace API.Extensions;

public static class TmsDataContextExtensions
{
    public static void SeedData(this TmsDataContext context)
    {
        if (context.Drivers.Any()) return;
        context.Drivers.AddRange(
            new Driver
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FirstName = "Іван",
                LastName = "Петренко",
                Phone = "+380501234567",
                Email = "ivan.petrenko@example.com",
                Note = "Досвідчений водій",
                RegistrationDate = DateTime.UtcNow
            },
            new Driver
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                FirstName = "Олексій",
                LastName = "Коваленко",
                Phone = "+380671112233",
                Email = "oleksii.kovalenko@example.com",
                Note = null,
                RegistrationDate = DateTime.UtcNow
            }
        );
        context.SaveChanges();
    }
}