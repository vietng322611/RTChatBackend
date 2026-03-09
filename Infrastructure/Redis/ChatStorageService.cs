using System.Text.Json;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class ChatStorageService(RedisConnectionFactory factory): IChatStorageService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();
    
    public async Task<Chat> GetOrCreateChatAsync(Guid uid1, Guid uid2)
    {
        var pairKey = $"chat:pair:{uid1}:{uid2}";

        var existingValue = await _redis.StringGetAsync(pairKey);
        if (existingValue.HasValue)
        {
            var existingChat = JsonSerializer.Deserialize<Chat>(existingValue.ToString());
            return existingChat ?? 
                   throw new Exception("Stored chat payload is invalid.");
        }

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Uid1 = uid1,
            Uid2 = uid2,
            CreatedAt = DateTime.UtcNow
        };

        var serialized = JsonSerializer.Serialize(chat);

        var created = await _redis.StringSetAsync(pairKey, serialized, when: When.NotExists);
        if (created)
        {
            await _redis.SetAddAsync($"user-chats:{uid1}", chat.Id.ToString());
            await _redis.SetAddAsync($"user-chats:{uid2}", chat.Id.ToString());
            await _redis.StringSetAsync($"chat:id:{chat.Id}", serialized);

            return chat;
        }

        var winnerValue = await _redis.StringGetAsync(pairKey);
        if (!winnerValue.HasValue)
        {
            throw new Exception("Chat was not found after creation attempt.");
        }

        var winnerChat = JsonSerializer.Deserialize<Chat>(winnerValue.ToString());
        return winnerChat 
               ?? throw new Exception("Stored chat payload is invalid.");
    }

    public Task RemoveChatAsync(Guid chatId)
    {
        throw new NotImplementedException();
    }
}