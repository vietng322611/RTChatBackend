using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class UserService: IUserService
{
    public Task<UserDto> CreateAsync(string username)
    {
        throw new NotImplementedException();
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