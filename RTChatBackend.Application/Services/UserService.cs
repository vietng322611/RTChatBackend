using System.Text.Json;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Services;

public class UserService(
    IUserSessionService userSession,
    ICodeGenerator codeGenerator)
    : IUserService
{
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

        await userSession.SetTemporaryUserAsync(user.UserId, data);
        await userSession.SetUsernameMappingAsync(user.Username, user.UserId);
        await userSession.SetLoginCodeMappingAsync(user.LoginCode, user.UserId);

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

    public async Task<List<string>> SearchAsync(string query)
    {
        if (query != string.Empty && string.IsNullOrWhiteSpace(query))
            return [];

        var usernames = await userSession.GetAllUsernamesAsync();

        if (string.IsNullOrEmpty(query))
            return usernames.OrderBy(u => u).ToList();

        return usernames
            .Where(u => u.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.Equals(query, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(u => u.StartsWith(query, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(u => u.Length)
            .ThenBy(u => u)
            .ToList();
    }
}