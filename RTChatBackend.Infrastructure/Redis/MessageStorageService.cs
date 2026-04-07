using System.Text.Json;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;
using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class MessageStorageService(
    RedisConnectionFactory factory,
    RedisOptions options) : IMessageStorageService
{
    private readonly TimeSpan _messageTtl = TimeSpan.FromMinutes(options.Ttl);
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
        var created = await _redis.StringSetAsync($"message:{messageId}", serialized, _messageTtl, When.NotExists);
        if (!created) throw new Exception("Failed to store message.");

        var chatMessagesKey = $"chat:messages:{chatId}";
        await _redis.SetAddAsync(chatMessagesKey, messageId.ToString());
        await _redis.KeyExpireAsync(chatMessagesKey, _messageTtl);

        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        var chatMessagesKey = $"chat:messages:{chatId}";

        // Refresh TTL on read
        await _redis.KeyExpireAsync(chatMessagesKey, _messageTtl);

        var messageIds = await _redis.SetMembersAsync(chatMessagesKey);

        if (messageIds.Length == 0) return [];

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
                // Refresh message TTL
                await _redis.KeyExpireAsync($"message:{message.Id}", _messageTtl);
                messages.Add(message);
            }
        }

        return messages;
    }
}