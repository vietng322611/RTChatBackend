using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class RedisConnectionFactory
{
    private readonly IConnectionMultiplexer _connection;

    public RedisConnectionFactory(string connectionString)
    {
        _connection = ConnectionMultiplexer.Connect(connectionString);
    }

    public RedisConnectionFactory(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public IConnectionMultiplexer Connection => _connection;
}