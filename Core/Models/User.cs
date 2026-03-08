namespace RTChatBackend.Core.Models;

public class User
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public string LoginCode { get; set; }
}