namespace RTChatBackend.Application.Interfaces;

public interface IUserSessionService
{
    Task SetTemporaryUserAsync(Guid userId, string data);
    Task<bool> IsUsernameTakenAsync(string username);
    Task SetUsernameMappingAsync(string username, Guid userId);
    Task SetLoginCodeMappingAsync(string loginCode, Guid userId);
    Task<Guid?> GetUserIdByUsernameAsync(string username);
    Task<Guid?> GetUserIdByLoginCodeAsync(string loginCode);
    Task<string?> GetUserDataAsync(Guid userId);
    Task<List<string>> GetAllUsernameAsync();
}