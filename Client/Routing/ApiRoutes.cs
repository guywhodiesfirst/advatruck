namespace Client.Routing;

public static class ApiRoutes
{
    private const string V1 = "v1";

    private const string Version = V1;

    public static class Auth
    {
        public static string Login => $"api/{Version}/auth/login";
    }

    public static class Loads
    {
        public static string GetAll => $"api/{Version}/loads";

        public static string Create => $"api/{Version}/loads";

        public static string Update(Guid id) => $"api/{Version}/loads/{id}";

        public static string GetById(Guid id)
            => $"api/{Version}/loads/{id}";

        public static string Delete(Guid id)
            => $"api/{Version}/loads/{id}";

        public static string UpdateStatus(Guid id)
            => $"api/{Version}/loads/{id}/status";

        public static string AssignDriver(Guid id)
            => $"api/{Version}/loads/{id}/assign-driver";

        public static string DeassignDriver(Guid id)
            => $"api/{Version}/loads/{id}/deassign-driver";
    }

    public static class AuctionLots
    {
        public static string GetAll => $"api/{Version}/auctionLots";

        public static string GetAllActive => $"api/{Version}/auctionLots/active";

        public static string Create => $"api/{Version}/auctionLots";

        public static string Update(Guid id) => $"api/{Version}/auctionLots/{id}";

        public static string GetById(Guid id)
            => $"api/{Version}/auctionLots/{id}";

        public static string Delete(Guid id)
            => $"api/{Version}/auctionLots/{id}";

        public static string UpdateStatus(Guid id)
            => $"api/{Version}/auctionLots/{id}/status";
    }

    public static class Bids
    {
        public static string GetAll => $"api/{Version}/bids";

        public static string Place => $"api/{Version}/bids";

        public static string GetById(Guid id)
            => $"api/{Version}/bids/{id}";

        public static string Delete(Guid id)
            => $"api/{Version}/bids/{id}";
    }

    public static class Drivers
    {
        public static string GetAll => $"api/{Version}/drivers";

        public static string GetMe => $"api/{Version}/drivers/me";

        public static string GetById(Guid id) => $"api/{Version}/drivers/{id}";

        public static string Update(Guid id) => $"api/{Version}/drivers/{id}";

        public static string GetActiveLoad(Guid id) => $"api/{Version}/drivers/{id}/active-load";

        public static string GetProfileById(Guid id) => $"api/{Version}/drivers/{id}/profile";
    }

    public static class Users
    {
        public static string GetAll => $"api/{Version}/users";

        public static string Register => $"api/{Version}/auth/register";
    }

    public static class Admins
    {
        public static string GetMe => $"api/{Version}/admins/me";
    }

    public static class Dispatchers
    {
        public static string GetMe => $"api/{Version}/dispatchers/me";
    }

    public static class Vehicles
    {
        public static string GetAll => $"api/{Version}/vehicles";

        public static string Create => $"api/{Version}/vehicles";

        public static string GetById(Guid id) => $"api/{Version}/vehicles/{id}";

        public static string Update(Guid id) => $"api/{Version}/vehicles/{id}";

        public static string Delete(Guid id) => $"api/{Version}/vehicles/{id}";
    }
}