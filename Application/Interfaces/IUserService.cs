using RTChatBackend.Application.DTOs;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Interfaces;

public interface IUserService
{
    Task<User?> CreateAsync(string username); // Return User object because UserDto don't contain LoginCode
    Task<UserDto?> LoginAsync(string loginCode);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<List<UserDto>> SearchAsync(string username);
}