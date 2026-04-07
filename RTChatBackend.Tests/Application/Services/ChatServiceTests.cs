using Moq;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Application.Services;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Tests.Application.Services;

public class ChatServiceTests
{
    private readonly Mock<IChatStorageService> _chatStorageMock;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _chatStorageMock = new Mock<IChatStorageService>();
        _chatService = new ChatService(_chatStorageMock.Object);
    }

    [Fact]
    public async Task GetAsync_WhenChatExists_ReturnsChatDto()
    {
        var chatId = Guid.NewGuid();
        var chat = new Chat
        {
            Id = chatId,
            Uid1 = Guid.NewGuid(),
            Uid2 = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        _chatStorageMock.Setup(s => s.GetAsync(chatId))
            .ReturnsAsync(chat);

        var result = await _chatService.GetAsync(chatId);

        Assert.NotNull(result);
        Assert.Equal(chat.Id, result.Id);
        Assert.Equal(chat.Uid1, result.User1Id);
    }

    [Fact]
    public async Task GetAsync_WhenChatDoesNotExist_ReturnsNull()
    {
        _chatStorageMock.Setup(s =>
            s.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Chat?)null);

        var result = await _chatService.GetAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrCreateAsync_ReturnsChatDto()
    {
        var uid1 = Guid.NewGuid();
        var uid2 = Guid.NewGuid();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Uid1 = uid1,
            Uid2 = uid2,
            CreatedAt = DateTime.UtcNow
        };
        _chatStorageMock.Setup(s =>
            s.GetOrCreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(chat);

        var result = await _chatService.GetOrCreateAsync(uid1, uid2);

        Assert.NotNull(result);
        Assert.Equal(chat.Id, result.Id);
    }

    [Fact]
    public async Task GetUserChatsAsync_ReturnsListOfChatDto()
    {
        var userId = Guid.NewGuid();
        var chats = new List<Chat>
        {
            new() { Id = Guid.NewGuid(), Uid1 = userId, Uid2 = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Uid1 = Guid.NewGuid(), Uid2 = userId }
        };
        _chatStorageMock.Setup(s => s.GetUserChatsAsync(userId)).ReturnsAsync(chats);

        var result = await _chatService.GetUserChatsAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == chats[0].Id);
        Assert.Contains(result, c => c.Id == chats[1].Id);
    }
}
