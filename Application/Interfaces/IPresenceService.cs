namespace RTChatBackend.Application.Interfaces;

public interface IPresenceService
{
    Task<bool> SetOnlineAsync(Guid userId, string connectionId);
    Task<bool> SetOfflineAsync(Guid userId, string connectionId);
    Task<bool> IsOnlineAsync(Guid userId);
}