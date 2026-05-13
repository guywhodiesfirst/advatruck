namespace API.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? user.FindFirst("sub")?.Value;

        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    public static Guid GetDispatcherId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst("dispatcher_id")?.Value;
        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    public static Guid GetDriverId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst("driver_id")?.Value;
        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    public static Guid GetAdminId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst("admin_id")?.Value;
        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    public static Guid GetEntityId(this ClaimsPrincipal user)
    {
        var entityId = user.FindFirst("dispatcher_id")?.Value
                       ?? user.FindFirst("driver_id")?.Value
                       ?? user.FindFirst("admin_id")?.Value;

        return entityId != null ? Guid.Parse(entityId) : Guid.Empty;
    }
}