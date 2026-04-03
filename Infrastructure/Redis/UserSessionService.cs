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

    public async Task SetUsernameMappingAsync(string username, Guid userId, TimeSpan expiry)
    {
        var normalized = username.ToLowerInvariant();

        await _redis.StringSetAsync($"username:{normalized}", userId.ToString(), expiry);
        await _redis.StringSetAsync($"username_display:{normalized}", username, expiry);
    }
    
    public Task<bool> IsUsernameTakenAsync(string username)
    {
        return _redis.KeyExistsAsync($"username:{username}");
    }

    public Task SetLoginCodeMappingAsync(string loginCode, Guid userId, TimeSpan expiry)
    {
        return _redis.StringSetAsync($"login-code:{loginCode}", userId.ToString(), expiry);
    }

    public async Task<Guid?> GetUserIdByUsernameAsync(string username)
    {
        var normalized = username.ToLowerInvariant();
        var value = await _redis.StringGetAsync($"username:{normalized}");
        return value.HasValue && Guid.TryParse(value.ToString(), out var userId)
            ? userId
            : null;
    }

    public async Task<Guid?> GetUserIdByLoginCodeAsync(string loginCode)
    {
        var value = await _redis.StringGetAsync($"login-code:{loginCode}");
        return value.HasValue && Guid.TryParse(value.ToString(), out var userId)
            ? userId
            : null;
    }

    public async Task<string?> GetUserDataAsync(Guid userId)
    {
        return await _redis.StringGetAsync($"temp-user:{userId}");
    }
}