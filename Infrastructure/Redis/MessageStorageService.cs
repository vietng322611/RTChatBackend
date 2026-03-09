using System.Text.Json;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class MessageStorageService(RedisConnectionFactory factory): IMessageStorageService
{
    private readonly IDatabase _redis = factory.Connection.GetDatabase();   
    
    public async Task<Message> SendMessageAsync(Guid chatId, Guid senderId, string content)
    {
        var messageId = Guid.NewGuid();
        var message = new Message
        {
            Id = messageId,
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        var serialized = JsonSerializer.Serialize(message);
        var created = await _redis.StringSetAsync($"message:{messageId}", serialized, when: When.NotExists);
        if (!created)
        {
            throw new Exception("Failed to store message.");
        }
        
        await _redis.SetAddAsync($"chat:messages:{chatId}", messageId.ToString());
        
        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        var chatMessagesKey = $"chat:messages:{chatId}";
        var messageIds = await _redis.ListRangeAsync(chatMessagesKey);

        if (messageIds.Length == 0)
        {
            return [];
        }

        var messageKeys = messageIds
            .Where(id => id.HasValue)
            .Select(id => (RedisKey)$"message:{id}")
            .ToArray();

        var messageValues = await _redis.StringGetAsync(messageKeys);

        var messages = new List<Message>(messageValues.Length);

        foreach (var value in messageValues.Where(v => v.HasValue))
        {
            var message = JsonSerializer.Deserialize<Message>(value.ToString());
            if (message is not null)
            {
                messages.Add(message);
            }
        }

        return messages;
    }
}