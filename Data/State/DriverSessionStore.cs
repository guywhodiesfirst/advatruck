namespace Data.State;

using Core.Models;
using Data.Interfaces;
using StackExchange.Redis;

/// <summary>
/// Redis-based implementation of driver session storage.
/// </summary>
public class DriverSessionStore(IConnectionMultiplexer redis) : IDriverSessionStore
{
    private readonly IDatabase _database = redis.GetDatabase();

    private static string TokenKey(Guid driverId) => $"dr:token:{driverId:D}";

    private static string ChatKey(Guid driverId) => $"dr:chat:{driverId:D}";

    private static string TrackingStateKey(Guid driverId) => $"dr:tr:state:{driverId:D}";

    private static string TrackingMessageKey(Guid driverId) => $"dr:tr:msg:{driverId:D}";

    private static string TrackingStoppedAtKey(Guid driverId) => $"dr:tr:stop:{driverId:D}";

    private static string LastLocationKey(Guid driverId) => $"dr:loc:last:{driverId:D}";

    private static string ChatToDriverKey(long chatId) => $"chat:dr:{chatId}";

    private static string AwaitingEmailKey(long chatId) => $"chat:awaiting_em:{chatId}";

    private static string AwaitingPasswordKey(long chatId) => $"chat:awaiting_pw:{chatId}";

    /// <inheritdoc />
    public async Task SaveBindingAsync(long chatId, Guid driverId, string token)
    {
        var t1 = _database.StringSetAsync(ChatToDriverKey(chatId), driverId.ToString("D"));
        var t2 = _database.StringSetAsync(ChatKey(driverId), chatId.ToString());
        var t3 = _database.StringSetAsync(TokenKey(driverId), token);

        await Task.WhenAll(t1, t2, t3);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetDriverIdByChatIdAsync(long chatId)
    {
        var result = await _database.StringGetAsync(ChatToDriverKey(chatId));
        return result.IsNullOrEmpty ? null : Guid.Parse(result!);
    }

    /// <inheritdoc />
    public async Task<string?> GetTokenByDriverIdAsync(Guid driverId)
    {
        return await _database.StringGetAsync(TokenKey(driverId));
    }

    /// <inheritdoc />
    public async Task<long?> GetChatIdByDriverIdAsync(Guid driverId)
    {
        var result = await _database.StringGetAsync(ChatKey(driverId));
        return result.IsNullOrEmpty ? null : long.Parse(result!);
    }

    /// <inheritdoc />
    public async Task ClearSessionAsync(long chatId)
    {
        var driverId = await GetDriverIdByChatIdAsync(chatId);

        if (driverId.HasValue)
        {
            var tasks = new List<Task>
            {
                _database.KeyDeleteAsync(TokenKey(driverId.Value)),
                _database.KeyDeleteAsync(ChatKey(driverId.Value)),
                _database.KeyDeleteAsync(TrackingStateKey(driverId.Value)),
                _database.KeyDeleteAsync(TrackingMessageKey(driverId.Value)),
                _database.KeyDeleteAsync(TrackingStoppedAtKey(driverId.Value)),
                _database.KeyDeleteAsync(LastLocationKey(driverId.Value)),
                _database.KeyDeleteAsync(ChatToDriverKey(chatId)),
            };
            await Task.WhenAll(tasks);
        }
        else
        {
            await _database.KeyDeleteAsync(ChatToDriverKey(chatId));
        }
    }

    /// <inheritdoc />
    public async Task MarkAwaitingEmailAsync(long chatId)
    {
        await _database.StringSetAsync(AwaitingEmailKey(chatId), "true", TimeSpan.FromMinutes(15));
    }

    /// <inheritdoc />
    public async Task<bool> IsAwaitingEmailAsync(long chatId)
    {
        var result = await _database.StringGetAsync(AwaitingEmailKey(chatId));
        return result.HasValue && result == "true";
    }

    /// <inheritdoc />
    public async Task ClearAwaitingEmailAsync(long chatId)
    {
        await _database.KeyDeleteAsync(AwaitingEmailKey(chatId));
    }

    /// <inheritdoc />
    public async Task SetTrackingActiveAsync(Guid driverId)
    {
        await _database.StringSetAsync(TrackingStateKey(driverId), "true");
    }

    /// <inheritdoc />
    public async Task<bool> IsTrackingActiveAsync(Guid driverId)
    {
        var result = await _database.StringGetAsync(TrackingStateKey(driverId));
        return result.HasValue && result == "true";
    }

    /// <inheritdoc />
    public async Task SaveTrackingMessageIdAsync(Guid driverId, int messageId)
    {
        await _database.StringSetAsync(TrackingMessageKey(driverId), messageId.ToString());
    }

    /// <inheritdoc />
    public async Task<int?> GetTrackingMessageIdAsync(Guid driverId)
    {
        var result = await _database.StringGetAsync(TrackingMessageKey(driverId));
        return result.IsNullOrEmpty ? null : int.Parse(result!);
    }

    /// <inheritdoc />
    public async Task ClearTrackingAsync(Guid driverId)
    {
        var t1 = _database.KeyDeleteAsync(TrackingStateKey(driverId));
        var t2 = _database.KeyDeleteAsync(TrackingMessageKey(driverId));

        await Task.WhenAll(t1, t2);
    }

    /// <inheritdoc />
    public async Task SaveLastLocationUpdateAsync(Guid driverId, DateTime time)
    {
        await _database.StringSetAsync(LastLocationKey(driverId), time.ToUniversalTime().Ticks.ToString());
    }

    /// <inheritdoc />
    public async Task<DateTime?> GetLastLocationUpdateAsync(Guid driverId)
    {
        var result = await _database.StringGetAsync(LastLocationKey(driverId));
        if (result.IsNullOrEmpty || !long.TryParse(result!, out var ticks))
        {
            return null;
        }

        return new DateTime(ticks, DateTimeKind.Utc);
    }

    /// <inheritdoc />
    public async Task SaveTrackingStoppedAtAsync(Guid driverId, DateTime time)
    {
        await _database.StringSetAsync(TrackingStoppedAtKey(driverId), time.ToUniversalTime().Ticks.ToString());
    }

    /// <inheritdoc />
    public async Task<DateTime?> GetTrackingStoppedAtAsync(Guid driverId)
    {
        var result = await _database.StringGetAsync(TrackingStoppedAtKey(driverId));
        if (result.IsNullOrEmpty || !long.TryParse(result!, out var ticks))
        {
            return null;
        }

        return new DateTime(ticks, DateTimeKind.Utc);
    }

    /// <inheritdoc />
    public async Task ClearTrackingStoppedAtAsync(Guid driverId)
    {
        await _database.KeyDeleteAsync(TrackingStoppedAtKey(driverId));
    }

    /// <inheritdoc />
    public async Task MarkAwaitingPasswordAsync(long chatId, string email)
    {
        await _database.StringSetAsync(AwaitingPasswordKey(chatId), email, TimeSpan.FromMinutes(10));
    }

    /// <inheritdoc />
    public async Task<PasswordAwaitState> GetAwaitingPasswordStateAsync(long chatId)
    {
        var result = await _database.StringGetAsync(AwaitingPasswordKey(chatId));

        return new PasswordAwaitState
        {
            IsAwaiting = result.HasValue,
            Email = result.ToString(),
        };
    }

    /// <inheritdoc />
    public async Task ClearAwaitingPasswordAsync(long chatId)
    {
        await _database.KeyDeleteAsync(AwaitingPasswordKey(chatId));
    }
}