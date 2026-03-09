namespace RTChatBackend.Core.Models;

public class Chat
{
    public Guid Id { get; set; }
    public Guid Uid1 { get; set; }
    public Guid Uid2 { get; set; }
    public DateTime CreatedAt { get; set; }
}