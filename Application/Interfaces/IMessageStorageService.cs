using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Interfaces;

public interface IMessageStorageService
{
    Task<Message> SendAsync(Guid chatId, Guid senderId, string content);
    Task<List<Message>> GetMessagesAsync(Guid chatId);
}