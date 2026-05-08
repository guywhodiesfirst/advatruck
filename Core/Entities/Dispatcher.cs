namespace Core.Entities;

using Core.Identity;

public class Dispatcher
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AppUser User { get; set; }

    public ICollection<Load>? Loads { get; set; }
}