using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Application.Services;

public class MessageService(
    IMessageStorageService messageStorage
    ): IMessageService
{
    public async Task<MessageDto> SendAsync(Guid chatId, Guid senderId, string content)
    {
        var message = await messageStorage.SendMessageAsync(chatId, senderId, content);
        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId)
    {
        var messages = await messageStorage.GetMessagesAsync(conversationId);
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChatId = m.ChatId,
            SenderId = m.SenderId,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList(); 
    }
}