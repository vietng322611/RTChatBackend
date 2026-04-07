using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RTChatBackend.Api.Controllers;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Test.Api.Controllers;

public class MessageControllerTests
{
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<IChatService> _chatServiceMock;
    private readonly MessageController _controller;

    public MessageControllerTests()
    {
        _messageServiceMock = new Mock<IMessageService>();
        _chatServiceMock = new Mock<IChatService>();
        _controller = new MessageController(_messageServiceMock.Object, _chatServiceMock.Object);
    }

    [Fact]
    public async Task GetMessages_UserNotMemberOfChat_ReturnsForbid()
    {
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var chat = new ChatDto { Id = chatId, User1Id = Guid.NewGuid(), User2Id = Guid.NewGuid() };

        _chatServiceMock.Setup(s => s.GetAsync(chatId)).ReturnsAsync(chat);

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ]));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.GetMessages(chatId);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetMessages_UserIsMember_ReturnsOk()
    {
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var chat = new ChatDto { Id = chatId, User1Id = userId, User2Id = Guid.NewGuid() };
        var messages = new List<MessageDto> { new() { Content = "Hi" } };

        _chatServiceMock.Setup(s => s.GetAsync(chatId)).ReturnsAsync(chat);
        _messageServiceMock.Setup(s => s.GetMessagesAsync(chatId)).ReturnsAsync(messages);

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ]));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.GetMessages(chatId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(messages, okResult.Value);
    }
}
