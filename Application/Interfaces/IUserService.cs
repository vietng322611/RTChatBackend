using RTChatBackend.Application.DTOs;

namespace RTChatBackend.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(string username);

    Task<UserDto?> GetByUsernameAsync(string username);

    Task<List<UserDto>> SearchAsync(string username);
}