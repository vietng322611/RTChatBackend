using RTChatBackend.Application.DTOs;

namespace RTChatBackend.Application.Interfaces;

public interface IChatService
{
    Task<ChatDto> GetOrCreateAsync(Guid user1Id, Guid user2Id);

    Task<List<ChatDto>> GetUserChatsAsync(Guid userId);
}