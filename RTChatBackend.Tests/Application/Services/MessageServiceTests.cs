using Moq;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Application.Services;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Tests.Application.Services;

public class MessageServiceTests
{
    private readonly Mock<IMessageStorageService> _messageStorageMock;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _messageStorageMock = new Mock<IMessageStorageService>();
        _messageService = new MessageService(_messageStorageMock.Object);
    }

    [Fact]
    public async Task SendAsync_ReturnsMessageDto()
    {
        var chatId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        const string content = "Hello!";
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        _messageStorageMock.Setup(s => s.SendMessageAsync(chatId, senderId, content)).ReturnsAsync(message);

        var result = await _messageService.SendAsync(chatId, senderId, content);

        Assert.Equal(message.Id, result.Id);
        Assert.Equal(content, result.Content);
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsListOfMessageDto()
    {
        var chatId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new() { Id = Guid.NewGuid(), ChatId = chatId, Content = "Msg 1" },
            new() { Id = Guid.NewGuid(), ChatId = chatId, Content = "Msg 2" }
        };
        _messageStorageMock.Setup(s => s.GetMessagesAsync(chatId)).ReturnsAsync(messages);

        var result = await _messageService.GetMessagesAsync(chatId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Msg 1", result[0].Content);
    }
}
