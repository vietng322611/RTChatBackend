using RTChatBackend.Application.Interfaces;
using RTChatBackend.Application.Services;
using RTChatBackend.Core.Models;
using System.Text.Json;

namespace RTChatBackend.Test.Application.Services;

public class UserServiceTests
{
    private readonly Mock<IUserSessionService> _userSessionMock;
    private readonly Mock<ICodeGenerator> _codeGeneratorMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userSessionMock = new Mock<IUserSessionService>();
        _codeGeneratorMock = new Mock<ICodeGenerator>();
        _userService = new UserService(_userSessionMock.Object, _codeGeneratorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_UsernameEmpty_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateAsync(""));
    }

    [Fact]
    public async Task CreateAsync_UsernameTaken_ReturnsNull()
    {
        _userSessionMock.Setup(s => s.IsUsernameTakenAsync("taken")).ReturnsAsync(true);

        var result = await _userService.CreateAsync("taken");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidUsername_ReturnsUserAndSavesToSession()
    {
        const string username = "new_user";
        const string loginCode = "123456";
        _userSessionMock.Setup(s => s.IsUsernameTakenAsync(username)).ReturnsAsync(false);
        _codeGeneratorMock.Setup(g => g.GenerateSessionCode()).Returns(loginCode);

        var result = await _userService.CreateAsync(username);

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Equal(loginCode, result.LoginCode);
        _userSessionMock.Verify(s => s.SetTemporaryUserAsync(result.UserId, It.IsAny<string>()), Times.Once);
        _userSessionMock.Verify(s => s.SetUsernameMappingAsync(username, result.UserId), Times.Once);
        _userSessionMock.Verify(s => s.SetLoginCodeMappingAsync(loginCode, result.UserId), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidCode_ReturnsNull()
    {
        _userSessionMock.Setup(s => s.GetUserIdByLoginCodeAsync("invalid")).ReturnsAsync((Guid?)null);

        var result = await _userService.LoginAsync("invalid");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCode_ReturnsUserDto()
    {
        const string loginCode = "valid";
        var userId = Guid.NewGuid();
        var user = new User
        {
            UserId = userId,
            Username = "test_user",
            LoginCode = loginCode
        };
        var userData = JsonSerializer.Serialize(user);

        _userSessionMock.Setup(s => s.GetUserIdByLoginCodeAsync(loginCode)).ReturnsAsync(userId);
        _userSessionMock.Setup(s => s.GetUserDataAsync(userId)).ReturnsAsync(userData);

        var result = await _userService.LoginAsync(loginCode);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("test_user", result.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_NotExists_ReturnsNull()
    {
        _userSessionMock.Setup(s => s.GetUserIdByUsernameAsync("nonexistent")).ReturnsAsync((Guid?)null);

        var result = await _userService.GetByUsernameAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_EmptyQuery_ReturnsAllSorted()
    {
        var usernames = new List<string> { "Charlie", "Alice", "Bob" };
        _userSessionMock.Setup(s => s.GetAllUsernamesAsync()).ReturnsAsync(usernames);

        var result = await _userService.SearchAsync("");

        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0]);
        Assert.Equal("Bob", result[1]);
        Assert.Equal("Charlie", result[2]);
    }

    [Fact]
    public async Task SearchAsync_WithQuery_ReturnsFilteredAndSorted()
    {
        var usernames = new List<string> { "Alice", "Alicia", "Bob", "Malice" };
        _userSessionMock.Setup(s => s.GetAllUsernamesAsync()).ReturnsAsync(usernames);

        var result = await _userService.SearchAsync("Ali");

        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0]);
        Assert.Equal("Alicia", result[1]);
        Assert.Equal("Malice", result[2]);
    }
}
