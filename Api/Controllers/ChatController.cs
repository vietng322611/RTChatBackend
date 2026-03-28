using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Api.Controllers;

[ApiController]
[Route("api/chats/")]
public class ChatController(
    IChatService chatService
    ): ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserChats()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var chats = await chatService.GetUserChatsAsync(userId);
        return Ok(chats);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid uid2)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        if (userId == uid2)
        {
            return BadRequest("Cannot create a chat with yourself.");
        }

        var chat = await chatService.GetOrCreateAsync(userId, uid2);
        return Ok(chat);
    }
}