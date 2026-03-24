using RTChatBackend.Core.Models;

namespace RTChatBackend.Application.Interfaces;

public interface IChatStorageService
{
    Task<Chat> GetOrCreateAsync(Guid uid1, Guid uid2);
}