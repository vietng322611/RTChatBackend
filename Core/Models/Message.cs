namespace RTChatBackend.Core.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}