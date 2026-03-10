namespace RTChatBackend.Application.Interfaces;

public interface IUserSessionService
{
    Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry);
    Task<bool> IsUsernameTakenAsync(string username);
    Task SetUsernameMappingAsync(string username, Guid userId, TimeSpan expiry);
    Task SetLoginCodeMappingAsync(string loginCode, Guid userId, TimeSpan expiry);
    Task<Guid?> GetUserIdByUsernameAsync(string username);
    Task<Guid?> GetUserIdByLoginCodeAsync(string loginCode);
    Task<string?> GetUserDataAsync(Guid userId);
}