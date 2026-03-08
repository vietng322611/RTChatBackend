using System.Text.Json;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Services;

public class UserService: IUserService
{
    private readonly IUserSessionService _userSessionService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly int _sessionTtl;

    public UserService(
        IConfiguration config,
        IUserSessionService userSessionService,
        ICodeGenerator codeGenerator)
    {
        if (!int.TryParse(config["Redis:SessionTtl"], out _sessionTtl))
        {
            throw new ArgumentException("Invalid SessionTtl", nameof(config));
        }
        _userSessionService = userSessionService;
        _codeGenerator = codeGenerator;
    }
    
    public async Task<UserDto> CreateAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            LoginCode = _codeGenerator.GenerateSessionCode()
        };

        var data = JsonSerializer.Serialize(user);
        
        await _userSessionService.SetTemporaryUserAsync(
            user.UserId,
            data,
            TimeSpan.FromMinutes(_sessionTtl));

        return new UserDto
        {
            Username = user.Username,
            LoginCode = user.LoginCode
        };
    }

    public Task<UserDto?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDto>> SearchAsync(string username)
    {
        throw new NotImplementedException();
    }
}