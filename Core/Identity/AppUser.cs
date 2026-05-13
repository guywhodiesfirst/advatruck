namespace Core.Identity;

using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser<Guid>
{
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;

    [MaxLength(50)]
    public string LastName { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public Driver? Driver { get; set; }

    public Dispatcher? Dispatcher { get; set; }

    public Admin? Admin { get; set; }
}