using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Api.Controllers;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Test.Api.Controllers;

public class ChatControllerTests
{
    private readonly Mock<IChatService> _chatServiceMock;
    private readonly ChatController _controller;

    public ChatControllerTests()
    {
        _chatServiceMock = new Mock<IChatService>();
        _controller = new ChatController(_chatServiceMock.Object);
    }

    [Fact]
    public async Task GetUserChats_ReturnsOkWithChats()
    {
        var userId = Guid.NewGuid();
        var chats = new List<ChatDto> { new() { Id = Guid.NewGuid() } };
        _chatServiceMock.Setup(s => s.GetUserChatsAsync(userId)).ReturnsAsync(chats);

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ]));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.GetUserChats();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(chats, okResult.Value);
    }

    [Fact]
    public async Task Create_SameUser_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ]));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.Create(userId);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
