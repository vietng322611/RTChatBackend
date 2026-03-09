using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Interfaces;

public interface IChatStorageService
{
    Task<Chat> GetOrCreateChatAsync(Guid uid1, Guid uid2);
    Task RemoveChatAsync(Guid chatId);
}