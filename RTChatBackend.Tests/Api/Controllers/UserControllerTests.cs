using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RTChatBackend.Api.Controllers;
using RTChatBackend.Api.DTOs;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;
using RTChatBackend.Core.Models;

namespace RTChatBackend.Tests.Api.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetByUsername_Exists_ReturnsOk()
    {
        const string username = "test1";
        var userDto = new UserDto { Username = username };
        _userServiceMock.Setup(s => s.GetByUsernameAsync(username))
            .ReturnsAsync(userDto);

        var result = await _controller.GetByUsername(username);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, okResult.Value);
    }

    [Fact]
    public async Task GetByUsername_NotExists_ReturnsNotFound()
    {
        _userServiceMock.Setup(s => 
            s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto?)null);

        var result = await _controller.GetByUsername("nonexistent");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Register_Success_ReturnsCreatedAtAction()
    {
        var request = new CreateUserRequest { Username = "new_user" };
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "new_user",
            LoginCode = "XYZ"
        };
        _userServiceMock.Setup(s => s.CreateAsync(request.Username))
            .ReturnsAsync(user);

        var result = await _controller.Register(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UserController.GetByUsername), createdResult.ActionName);
        Assert.Equal(user, createdResult.Value);
    }

    [Fact]
    public async Task Register_UsernameExists_ReturnsConflict()
    {
        var request = new CreateUserRequest { Username = "existing" };
        _userServiceMock.Setup(s => s.CreateAsync(request.Username))
            .ReturnsAsync((User?)null);

        var result = await _controller.Register(request);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCode_ReturnsOk()
    {
        var request = new LoginRequest { LoginCode = "123456" };
        var userDto = new UserDto { UserId = Guid.NewGuid(), Username = "test_user" };
        _userServiceMock.Setup(s => s.LoginAsync(request.LoginCode))
            .ReturnsAsync(userDto);

        var authServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = serviceProviderMock.Object }
        };

        var result = await _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, okResult.Value);
    }

    [Fact]
    public async Task Login_InvalidCode_ReturnsUnauthorized()
    {
        var request = new LoginRequest { LoginCode = "wrong" };
        _userServiceMock.Setup(s => s.LoginAsync(request.LoginCode))
            .ReturnsAsync((UserDto?)null);

        var result = await _controller.Login(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Search_ReturnsOkWithList()
    {
        var usernames = new List<string> { "Alice", "Bob" };
        _userServiceMock.Setup(s => s.SearchAsync("test"))
            .ReturnsAsync(usernames);

        var result = await _controller.Search("test");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(usernames, okResult.Value);
    }
}
