namespace RTChatBackend.Application.Interfaces;

public interface IUserSessionService
{
    Task SetTemporaryUserAsync(Guid userId, string data, TimeSpan expiry);
}