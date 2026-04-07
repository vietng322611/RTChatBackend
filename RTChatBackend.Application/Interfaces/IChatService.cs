using RTChatBackend.Application.DTOs;

namespace RTChatBackend.Application.Interfaces;

public interface IChatService
{
    Task<ChatDto?> GetAsync(Guid chatId);
    Task<ChatDto> GetOrCreateAsync(Guid uid1, Guid uid2);
    Task<List<ChatDto>> GetUserChatsAsync(Guid userId);
}