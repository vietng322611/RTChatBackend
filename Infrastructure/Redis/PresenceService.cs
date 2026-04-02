using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class PresenceService(
    RedisConnectionFactory factory,
    RedisOptions options) : IPresenceService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();
    private readonly TimeSpan _presenceTtl = TimeSpan.FromMinutes(options.Ttl);

    public async Task<bool> SetOnlineAsync(
        Guid userId,
        string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
        {
            throw new ArgumentException("Connection ID cannot be null or empty.", nameof(connectionId));
        }
        
        var key = $"online:{userId}";
        await _redis.HashSetAsync(key, connectionId, _presenceTtl.ToString("o"));

        // Refresh TTL if still online
        if (_presenceTtl != TimeSpan.Zero)
        {
            await _redis.KeyExpireAsync(key, _presenceTtl);
        }
        
        var connectionCount = await _redis.HashLengthAsync(key);
        return connectionCount == 1;  // True if just went from 0 to 1
    }

    public async Task<bool> SetOfflineAsync(Guid userId, string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
        {
            throw new ArgumentException("Connection ID cannot be null or empty.", nameof(connectionId));
        }

        var key = $"online:{userId}";

        await _redis.HashDeleteAsync(key, connectionId);

        // Check remaining connections
        var connectionCount = await _redis.HashLengthAsync(key);
        if (connectionCount == 0)
        {
            await _redis.KeyDeleteAsync(key);
            return true;
        }

        // Refresh TTL if still online
        if (_presenceTtl != TimeSpan.Zero && connectionCount > 0)
        {
            await _redis.KeyExpireAsync(key, _presenceTtl);
        }

        return false;  // Still online
    }
    
    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        var connectionCount = await _redis.HashLengthAsync($"online:{userId}");
        return connectionCount > 0;
    }
}