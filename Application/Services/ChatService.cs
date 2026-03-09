using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class ChatService(
    IChatStorageService chatStorage
    ): IChatService
{
    public async Task<ChatDto> GetOrCreateAsync(Guid uid1, Guid uid2)
    {
        // Skip verify userIds here
        var firstUid = uid1.CompareTo(uid2) < 0 ? uid1 : uid2;
        var secondUid = uid1.CompareTo(uid2) < 0 ? uid2 : uid1;

        var chat = await chatStorage.GetOrCreateChatAsync(firstUid, secondUid);
        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.Uid1,
            User2Id = chat.Uid2,
            CreatedAt = chat.CreatedAt
        };
    }

    public Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}