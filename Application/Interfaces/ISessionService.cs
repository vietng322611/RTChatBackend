namespace RTChatBackend.Application.Interfaces;

public interface ISessionService
{
    Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry);

    Task<string?> GetTemporaryUserAsync(Guid userId);
}