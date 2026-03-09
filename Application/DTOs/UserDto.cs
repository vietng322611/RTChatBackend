namespace RTChatBackend.Application.DTOs;

public sealed class UserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string LoginCode { get; set; } = "";
}