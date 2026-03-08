using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class OnlineUserTracker(RedisConnectionFactory factory) : IOnlineUserTracker
{
    private readonly IDatabase _db = factory.Connection.GetDatabase();

    public Task SetOnlineAsync(Guid userId, TimeSpan ttl)
    {
        return _db.StringSetAsync($"online:{userId}", "1", ttl);
    }

    public async Task<bool> IsOnlineAsync(Guid userId)
    {
        return await _db.KeyExistsAsync($"online:{userId}");
    }
}