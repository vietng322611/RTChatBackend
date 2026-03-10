using System.Text.Json;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Services;

public class UserService: IUserService
{
    private readonly IUserSessionService _userSession;
    private readonly ICodeGenerator _codeGenerator;
    private readonly int _sessionTtl;

    public UserService(
        IConfiguration config,
        IUserSessionService userSession,
        ICodeGenerator codeGenerator)
    {
        if (!int.TryParse(config["Redis:SessionTtl"], out _sessionTtl))
        {
            throw new ArgumentException("Invalid SessionTtl", nameof(config));
        }
        _userSession = userSession;
        _codeGenerator = codeGenerator;
    }
    
    public async Task<UserDto?> CreateAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        }

        if (await _userSession.IsUsernameTakenAsync(username))
        {
            return null; // Or throw an exception, but DTO? returns null usually means not found or conflict in this context
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            LoginCode = _codeGenerator.GenerateSessionCode()
        };

        var data = JsonSerializer.Serialize(user);
        var expiry = TimeSpan.FromMinutes(_sessionTtl);
        
        await _userSession.SetTemporaryUserAsync(
            user.UserId,
            data,
            expiry);
        
        await _userSession.SetUsernameMappingAsync(
            user.Username,
            user.UserId,
            expiry);

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            LoginCode = user.LoginCode
        };
    }

    public Task<UserDto?> LoginAsync(string loginCode)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var userId = await _userSession.GetUserIdByUsernameAsync(username);
        if (userId == null) return null;

        var userData = await _userSession.GetUserDataAsync(userId.Value);
        if (userData == null) return null;

        var user = JsonSerializer.Deserialize<User>(userData);
        if (user == null) return null;

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            LoginCode = user.LoginCode
        };
    }

    public Task<List<UserDto>> SearchAsync(string username)
    {
        throw new NotImplementedException();
    }
}