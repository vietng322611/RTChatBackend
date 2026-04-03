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

    public async Task<List<string>> GetAllUsernameAsync()
    {
        var toRemove = new List<RedisValue>(); // batch removal
        
        var result = new List<string>();

        var usernames = await _redis.SetMembersAsync("usernames");
        foreach (var usernameValue in usernames)
        {
            var username = usernameValue.ToString();
            var normalized = username.ToLowerInvariant();

            var userIdValue = await _redis.StringGetAsync($"username:{normalized}");
            if (!userIdValue.HasValue)
            {
                toRemove.Add(usernameValue); // stage for removal
                continue;
            }

            result.Add(username);
        }
        
        await _redis.SetRemoveAsync("usernames", toRemove.ToArray());

        return result;
    }
}