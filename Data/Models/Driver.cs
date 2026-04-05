namespace Data.Models;

public class Driver
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Note { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public DriverLocation DriverLocation { get; set; } = null!;
}
