namespace RTChatBackend.Application.Interfaces;

public interface IPresenceService
{
    Task SetOnlineAsync(Guid userId, TimeSpan ttl);
    Task SetOfflineAsync(Guid userId);
    Task<bool> IsOnlineAsync(Guid userId);
}