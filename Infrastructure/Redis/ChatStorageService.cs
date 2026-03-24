using System.Text.Json;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class ChatStorageService(
    RedisConnectionFactory factory,
    RedisOptions options) : IChatStorageService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();
    private readonly TimeSpan _chatTtl = TimeSpan.FromMinutes(options.Ttl);
    
    public async Task<Chat> GetOrCreateAsync(Guid uid1, Guid uid2)
    {
        // Ids order will not be handled here
        var pairKey = $"chat:pair:{uid1}:{uid2}";

        var existingValue = await _redis.StringGetAsync(pairKey);
        if (existingValue.HasValue)
        {
            var existingChat = JsonSerializer.Deserialize<Chat>(existingValue.ToString());
            
            // Refresh TTL
            await _redis.KeyExpireAsync(pairKey, _chatTtl);
            var chatData = existingChat ?? throw new Exception("Stored chat payload is invalid.");
            await _redis.KeyExpireAsync($"chat:id:{chatData.Id}", _chatTtl);
            await _redis.KeyExpireAsync($"user-chats:{uid1}", _chatTtl);
            await _redis.KeyExpireAsync($"user-chats:{uid2}", _chatTtl);
            
            return chatData;
        }

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Uid1 = uid1,
            Uid2 = uid2,
            CreatedAt = DateTime.UtcNow
        };

        var serialized = JsonSerializer.Serialize(chat);

        var created = await _redis.StringSetAsync(pairKey, serialized, _chatTtl, when: When.NotExists);
        if (created)
        {
            await _redis.SetAddAsync($"user-chats:{uid1}", chat.Id.ToString());
            await _redis.KeyExpireAsync($"user-chats:{uid1}", _chatTtl);
            
            await _redis.SetAddAsync($"user-chats:{uid2}", chat.Id.ToString());
            await _redis.KeyExpireAsync($"user-chats:{uid2}", _chatTtl);
            
            await _redis.StringSetAsync($"chat:id:{chat.Id}", serialized, _chatTtl);

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
}