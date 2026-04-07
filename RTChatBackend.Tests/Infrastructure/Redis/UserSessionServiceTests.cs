using System.Text.Json;
using Moq;
using RTChatBackend.Core.Models;
using RTChatBackend.Infrastructure.Redis;
using StackExchange.Redis;

namespace RTChatBackend.Tests.Infrastructure.Redis;

public class UserSessionServiceTests
{
    private readonly Mock<IDatabase> _dbMock;
    private readonly Mock<IConnectionMultiplexer> _multiplexerMock;
    private readonly UserSessionService _service;
    private readonly RedisOptions _options;

    public UserSessionServiceTests()
    {
        _dbMock = new Mock<IDatabase>();
        _multiplexerMock = new Mock<IConnectionMultiplexer>();
        _multiplexerMock.Setup(m => 
            m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_dbMock.Object);

        var factory = new RedisConnectionFactory(_multiplexerMock.Object);
        _options = new RedisOptions { Ttl = 15 };
        _service = new UserSessionService(factory, _options);
    }

    [Fact]
    public async Task SetTemporaryUserAsync_CallsStringSetWithTtl()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            UserId = userId,
            Username = "test_user",
            LoginCode = "valid"
        };
        var userData = JsonSerializer.Serialize(user);

        await _service.SetTemporaryUserAsync(user.UserId, userData);

        _dbMock.Verify(db => db.StringSetAsync(
            (RedisKey)$"temp-user:{userId}",
            userData,
            It.Is<TimeSpan>(t => Math.Abs(t.TotalMinutes - _options.Ttl) < 0.05),
            false,
            When.Always,
            CommandFlags.None), Times.Once);
    }

    [Fact]
    public async Task IsUsernameTakenAsync_CallsKeyExists()
    {
        const string username = "test_user";
        _dbMock.Setup(db => 
            db.KeyExistsAsync((RedisKey)$"username:{username}", CommandFlags.None))
            .ReturnsAsync(true);

        var result = await _service.IsUsernameTakenAsync(username);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserIdByUsernameAsync_ReturnsGuid_WhenExists()
    {
        const string username = "test_user";
        var userId = Guid.NewGuid();
        _dbMock.Setup(db => 
                db.StringGetAsync((RedisKey)$"username:{username.ToLowerInvariant()}", CommandFlags.None))
            .ReturnsAsync(userId.ToString());

        var result = await _service.GetUserIdByUsernameAsync(username);

        Assert.Equal(userId, result);
    }
}
