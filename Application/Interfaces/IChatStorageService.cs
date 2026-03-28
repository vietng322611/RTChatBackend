using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Interfaces;

public interface IChatStorageService
{
    Task<Chat?> GetAsync(Guid chatId);
    Task<Chat> GetOrCreateAsync(Guid uid1, Guid uid2);
    Task<List<Chat>> GetUserChatsAsync(Guid userId);
}