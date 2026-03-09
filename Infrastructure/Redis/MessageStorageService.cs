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

    public Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        throw new NotImplementedException();
    }
}