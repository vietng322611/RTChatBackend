namespace RTChatBackend.Infrastructure.Redis;

public static class RedisKeys
{
    public static string Session(string code)
        => $"session:{code}";

    public static string Messages(string code)
        => $"messages:{code}";
}