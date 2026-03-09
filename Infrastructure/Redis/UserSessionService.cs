using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class UserSessionService(RedisConnectionFactory factory) : IUserSessionService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();

    public Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry)
    {
        return _redis.StringSetAsync($"temp-user:{userId}", data, expiry);
    }
}