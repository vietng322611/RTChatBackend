using RTChatBackend.Application.DTOs;

namespace RTChatBackend.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendAsync(
        Guid chatId,
        Guid senderId,
        string content);

    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId);
}