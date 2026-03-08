using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class ChatService: IChatService
{
    public Task<ChatDto> GetOrCreateAsync(Guid user1Id, Guid user2Id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}