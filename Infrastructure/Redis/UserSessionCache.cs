using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class UserSessionCache(RedisConnectionFactory factory) : IUserSessionCache
{
    private readonly IDatabase _db = factory.Connection.GetDatabase();

    public Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry)
    {
        return _db.StringSetAsync($"temp-user:{userId}", data, expiry);
    }

    public async Task<string?> GetTemporaryUserAsync(Guid userId)
    {
        return await _db.StringGetAsync($"temp-user:{userId}");
    }
}
}