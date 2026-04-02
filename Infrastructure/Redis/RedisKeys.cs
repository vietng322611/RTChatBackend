namespace RTChatBackend.Infrastructure.Redis;

public static class RedisKeys
{
    public static string Session(string code)
    {
        return $"session:{code}";
    }

    public static string Messages(string code)
    {
        return $"messages:{code}";
    }
}