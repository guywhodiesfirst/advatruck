namespace Core.Entities;

using Core.Identity;

public class Admin
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AppUser User { get; set; }
}