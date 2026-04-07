using RTChatBackend.Application.Interfaces;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class UserSessionService(
    RedisConnectionFactory factory,
    RedisOptions options) : IUserSessionService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();
    private readonly TimeSpan _sessionTtl = TimeSpan.FromMinutes(options.Ttl);

    public Task SetTemporaryUserAsync(Guid userId, string data)
    {
        return _redis.StringSetAsync($"temp-user:{userId}", data, _sessionTtl);
    }

    public async Task SetUsernameMappingAsync(string username, Guid userId)
    {
        var normalized = username.ToLowerInvariant();

        await _redis.StringSetAsync($"username:{normalized}", userId.ToString(), _sessionTtl);
        
        // store display username
        await _redis.SetAddAsync("usernames", username);
    }
    
    public Task<bool> IsUsernameTakenAsync(string username)
    {
        return _redis.KeyExistsAsync($"username:{username}");
    }

    public Task SetLoginCodeMappingAsync(string loginCode, Guid userId)
    {
        return _redis.StringSetAsync($"login-code:{loginCode}", userId.ToString(), _sessionTtl);
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

    public async Task<List<string>> GetAllUsernamesAsync()
    {
        var usernames = await _redis.SetMembersAsync("usernames");
        if (usernames.Length == 0) return [];

        var keys = usernames
            .Select(u => (RedisKey)$"username:{u.ToString().ToLowerInvariant()}")
            .ToArray();
        var values = await _redis.StringGetAsync(keys);

        var result = new List<string>();
        var toRemove = new List<RedisValue>();

        for (var i = 0; i < usernames.Length; i++)
        {
            if (!values[i].HasValue)
            {
                toRemove.Add(usernames[i]);
                continue;
            }

            result.Add(usernames[i].ToString());
        }

        if (toRemove.Count > 0)
        {
            await _redis.SetRemoveAsync("usernames", toRemove.ToArray());
        }

        return result;
    }
}