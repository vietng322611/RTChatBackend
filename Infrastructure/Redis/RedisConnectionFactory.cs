using StackExchange.Redis;

namespace RTChatBackend.Infrastructure.Redis;

public class RedisConnectionFactory(string connectionString)
{
    private readonly Lazy<IConnectionMultiplexer> _connection = new(()
        => ConnectionMultiplexer.Connect(connectionString));

    public IConnectionMultiplexer Connection => _connection.Value;
}