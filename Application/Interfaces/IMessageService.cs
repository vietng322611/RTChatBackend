using RTChatBackend.Application.DTOs;

namespace RTChatBackend.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendAsync(
        Guid conversationId,
        Guid senderId,
        string content);

    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId);
}