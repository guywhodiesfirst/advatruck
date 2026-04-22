namespace Data.State;

using Data.Interfaces;
using StackExchange.Redis;

// TODO: refactor to use only driverId

/// <summary>
/// Redis-based implementation of driver session storage.
/// </summary>
public class DriverSessionStore(IConnectionMultiplexer redis) : IDriverSessionStore
{
    private readonly IDatabase _db = redis.GetDatabase();

    private static string TrackingStateKey(long chatId) => $"driver:tracking:state:{chatId}";

    private static string TrackingMessageKey(long chatId) => $"driver:tracking:message:{chatId}";

    private static string TrackingStoppedKey(long chatId) => $"driver:tracking:stopped:{chatId}";

    private static string AuthKey(long chatId) => $"driver:auth:{chatId}";

    private static string AwaitingEmailKey(long chatId) => $"driver:login:{chatId}";

    private static string LastLocationKey(Guid driverId) => $"driver:last-location:{driverId}";

    private static string DriverChatKey(Guid driverId) => $"driver:chat:{driverId}";

    /// <inheritdoc />
    public async Task SaveAuthenticatedDriverAsync(long chatId, Guid driverId)
    {
        await _db.StringSetAsync(AuthKey(chatId), driverId.ToString());
        await _db.StringSetAsync(DriverChatKey(driverId), chatId.ToString());
    }

    /// <inheritdoc />
    public async Task<Guid?> GetAuthenticatedDriverAsync(long chatId)
    {
        var value = await _db.StringGetAsync(AuthKey(chatId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return Guid.Parse(value!);
    }

    /// <inheritdoc />
    public async Task ClearAuthenticatedDriverAsync(long chatId)
    {
        await _db.KeyDeleteAsync(AuthKey(chatId));
    }

    /// <inheritdoc />
    public async Task MarkAwaitingEmailAsync(long chatId)
    {
        await _db.StringSetAsync(AwaitingEmailKey(chatId), "true");
    }

    /// <inheritdoc />
    public async Task<bool> IsAwaitingEmailAsync(long chatId)
    {
        var value = await _db.StringGetAsync(AwaitingEmailKey(chatId));
        return value.HasValue && value == "true";
    }

    /// <inheritdoc />
    public async Task ClearAwaitingEmailAsync(long chatId)
    {
        await _db.KeyDeleteAsync(AwaitingEmailKey(chatId));
    }

    /// <inheritdoc/>
    public async Task SaveTrackingMessageIdAsync(long chatId, int messageId)
    {
        await _db.StringSetAsync(TrackingMessageKey(chatId), messageId.ToString());
    }

    /// <inheritdoc/>
    public async Task<int?> GetTrackingMessageIdAsync(long chatId)
    {
        var value = await _db.StringGetAsync(TrackingMessageKey(chatId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return int.Parse(value!);
    }

    /// <inheritdoc />
    public async Task SetTrackingActiveAsync(long chatId)
    {
        await _db.StringSetAsync(TrackingStateKey(chatId), "true");
    }

    /// <inheritdoc />
    public async Task<bool> IsTrackingActiveAsync(long chatId)
    {
        var value = await _db.StringGetAsync(TrackingStateKey(chatId));
        return value.HasValue && value == "true";
    }

    /// <inheritdoc />
    public async Task ClearTrackingAsync(long chatId)
    {
        await _db.KeyDeleteAsync(TrackingStateKey(chatId));
        await _db.KeyDeleteAsync(TrackingMessageKey(chatId));
    }

    /// <inheritdoc />
    public async Task SaveLastLocationUpdateAsync(Guid driverId, DateTime time)
    {
        await _db.StringSetAsync(
            LastLocationKey(driverId),
            time.ToUniversalTime().Ticks.ToString());
    }

    /// <inheritdoc />
    public async Task<DateTime?> GetLastLocationUpdateAsync(Guid driverId)
    {
        var value = await _db.StringGetAsync(LastLocationKey(driverId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        if (!long.TryParse(value!, out var ticks))
        {
            return null;
        }

        return new DateTime(ticks, DateTimeKind.Utc);
    }

    /// <inheritdoc/>
    public async Task<long?> GetChatIdByDriverIdAsync(Guid driverId)
    {
        var value = await _db.StringGetAsync(DriverChatKey(driverId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return long.Parse(value!);
    }

    /// <inheritdoc />
    public async Task SaveTrackingStoppedAtAsync(long chatId, DateTime time)
    {
        await _db.StringSetAsync(
            TrackingStoppedKey(chatId),
            time.ToUniversalTime().Ticks.ToString());
    }

    /// <inheritdoc />
    public async Task<DateTime?> GetTrackingStoppedAtAsync(long chatId)
    {
        var value = await _db.StringGetAsync(TrackingStoppedKey(chatId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        if (!long.TryParse(value!, out var ticks))
        {
            return null;
        }

        return new DateTime(ticks, DateTimeKind.Utc);
    }

    public async Task ClearTrackingStoppedAtAsync(long chatId)
    {
        await _db.KeyDeleteAsync(TrackingStoppedKey(chatId));
    }
}