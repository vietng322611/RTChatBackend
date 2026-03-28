using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Api.Controllers;

[ApiController]
[Route("api/messages/")]
public class MessageController(
    IMessageService messageService,
    IChatService chatService
    ): ControllerBase
{
    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var chat = await chatService.GetAsync(chatId);
        if (chat == null)
        {
            return NotFound();
        }

        if (chat.User1Id != userId && chat.User2Id != userId)
        {
            return Forbid();
        }

        var messages = await messageService.GetMessagesAsync(chatId);
        return Ok(messages);
    }
}