namespace RTChatBackend.Infrastructure.Redis;

public class RedisOptions
{
    public string ConnectionString { get; set; } = null!;
    public int Ttl { get; set; } // One for all lmao
}