namespace RTChatBackend.Application.Interfaces;

public interface IConversationService
{
    Task<ConversationDto> GetOrCreateAsync(Guid user1Id, Guid user2Id);

    Task<List<ConversationDto>> GetUserChatsAsync(Guid userId);
}