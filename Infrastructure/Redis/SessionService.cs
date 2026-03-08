using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class SessionService(RedisConnectionFactory factory) : ISessionService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();

    public Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry)
    {
        return _redis.StringSetAsync($"temp-user:{userId}", data, expiry);
    }

    public async Task<string?> GetTemporaryUserAsync(Guid userId)
    {
        return await _redis.StringGetAsync($"temp-user:{userId}");
    }
}