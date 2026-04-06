using Bot.Interfaces;
using StackExchange.Redis;

namespace Bot.State;

/// <summary>
/// Redis-based implementation of driver session storage.
/// Stores authentication state and login flow state per Telegram chat.
/// </summary>
public class DriverSessionStore(IConnectionMultiplexer redis) : IDriverSessionStore
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static string TrackingStateKey(long chatId) => $"driver:tracking:state:{chatId}";
    private static string TrackingMessageKey(long chatId) => $"driver:tracking:message:{chatId}";
    private static string AuthKey(long chatId) => $"driver:auth:{chatId}";
    private static string AwaitingEmailKey(long chatId) => $"driver:login:{chatId}";

    /// <inheritdoc />
    public async Task SaveAuthenticatedDriverAsync(long chatId, Guid driverId)
    {
        await _db.StringSetAsync(AuthKey(chatId), driverId.ToString());
    }

    /// <inheritdoc />
    public async Task<Guid?> GetAuthenticatedDriverAsync(long chatId)
    {
        var value = await _db.StringGetAsync(AuthKey(chatId));

        if (value.IsNullOrEmpty)
            return null;

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
            return null;

        return int.Parse(value!);
    }

    public async Task SetTrackingActiveAsync(long chatId)
    {
        await _db.StringSetAsync(TrackingStateKey(chatId), "true");
    }

    public async Task<bool> IsTrackingActiveAsync(long chatId)
    {
        var value = await _db.StringGetAsync(TrackingStateKey(chatId));
        return value.HasValue && value == "true";
    }

    public async Task ClearTrackingAsync(long chatId)
    {
        await _db.KeyDeleteAsync(TrackingStateKey(chatId));
        await _db.KeyDeleteAsync($"driver:tracking_message:{chatId}");
    }
}