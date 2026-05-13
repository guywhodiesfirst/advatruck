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

        public static string Update => $"api/{Version}/loads";

        public static string GetById(Guid id)
            => $"api/{Version}/loads/{id}";

        public static string Delete(Guid id)
            => $"api/{Version}/loads/{id}";

        public static string UpdateStatus
            => $"api/{Version}/loads/status";

        public static string AssignDriver
            => $"api/{Version}/loads/assign-driver";
    }

    public static class AuctionLots
    {
        public static string GetAll => $"api/{Version}/auctionLots";

        public static string GetAllActive => $"api/{Version}/auctionLots/active";

        public static string Create => $"api/{Version}/auctionLots";

        public static string Update => $"api/{Version}/auctionLots";

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

        public static string Create => $"api/{Version}/bids";

        public static string Update => $"api/{Version}/bids";

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

        public static string Update => $"api/{Version}/drivers";

        public static string GetActiveLoad(Guid id) => $"api/{Version}/drivers/{id}/active-load";

        public static string GetLastLocation(Guid driverId)
            => $"api/{Version}/drivers/{driverId}/locations/last";
    }
}