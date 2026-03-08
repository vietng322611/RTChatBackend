using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class PresenceService(RedisConnectionFactory factory) : IPresenceService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();

    public Task SetOnlineAsync(Guid userId, TimeSpan ttl)
    {
        return _redis.StringSetAsync($"online:{userId}", "1", ttl);
    }

    public Task<bool> IsOnlineAsync(Guid userId)
    {
        return _redis.KeyExistsAsync($"online:{userId}");
    }

    public Task SetOfflineAsync(Guid userId)
    {
        return _redis.KeyDeleteAsync($"online:{userId}");
    }
}