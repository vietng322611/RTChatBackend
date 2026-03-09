namespace RTChatBackend.Api.DTOs;

public sealed class SendMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;w
}