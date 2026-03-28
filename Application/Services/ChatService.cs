using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class ChatService(
    IChatStorageService chatStorage
    ): IChatService
{
    public async Task<ChatDto?> GetAsync(Guid chatId)
    {
        var chat = await chatStorage.GetAsync(chatId);
        if (chat == null) return null;

        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.Uid1,
            User2Id = chat.Uid2,
            CreatedAt = chat.CreatedAt
        };
    }

    public async Task<ChatDto> GetOrCreateAsync(Guid uid1, Guid uid2)
    {
        // Skip verify userIds here
        var firstUid = uid1.CompareTo(uid2) < 0 ? uid1 : uid2;
        var secondUid = uid1.CompareTo(uid2) < 0 ? uid2 : uid1;

        var chat = await chatStorage.GetOrCreateAsync(firstUid, secondUid);
        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.Uid1,
            User2Id = chat.Uid2,
            CreatedAt = chat.CreatedAt
        };
    }

    public async Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        var chats = await chatStorage.GetUserChatsAsync(userId);
        return chats.Select(chat => new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.Uid1,
            User2Id = chat.Uid2,
            CreatedAt = chat.CreatedAt
        }).ToList();
    }
}