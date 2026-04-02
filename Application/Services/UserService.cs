using System.Text.Json;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;
using RTChatBackend.Infrastructure.Redis;

namespace RTChatBackend.Application.Services;

public class UserService(
    RedisOptions options,
    IUserSessionService userSession,
    ICodeGenerator codeGenerator)
    : IUserService
{
    private readonly TimeSpan _sessionTtl = TimeSpan.FromMinutes(options.Ttl);

    public async Task<User?> CreateAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (await userSession.IsUsernameTakenAsync(username)) return null;

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            LoginCode = codeGenerator.GenerateSessionCode()
        };

        var data = JsonSerializer.Serialize(user);

        await userSession.SetTemporaryUserAsync(
            user.UserId,
            data,
            _sessionTtl);

        await userSession.SetUsernameMappingAsync(
            user.Username,
            user.UserId,
            _sessionTtl);

        await userSession.SetLoginCodeMappingAsync(
            user.LoginCode,
            user.UserId,
            _sessionTtl);

        return user;
    }

    public async Task<UserDto?> LoginAsync(string loginCode)
    {
        if (string.IsNullOrWhiteSpace(loginCode)) return null;

        var userId = await userSession.GetUserIdByLoginCodeAsync(loginCode);
        if (userId == null) return null;

        var userData = await userSession.GetUserDataAsync(userId.Value);
        if (userData == null) return null;

        var user = JsonSerializer.Deserialize<User>(userData);
        if (user == null) return null;

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username
        };
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var userId = await userSession.GetUserIdByUsernameAsync(username);
        if (userId == null) return null;

        var userData = await userSession.GetUserDataAsync(userId.Value);
        if (userData == null) return null;

        var user = JsonSerializer.Deserialize<User>(userData);
        if (user == null) return null;

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username
        };
    }

    public Task<List<UserDto>> SearchAsync(string username)
    {
        throw new NotImplementedException();
    }
}