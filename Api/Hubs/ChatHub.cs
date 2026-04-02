using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Api.Hubs;

[Authorize]
public class ChatHub(
    IPresenceService presenceService,
    IMessageService messageService,
    IChatService chatService
    ): Hub
{
    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var becameOnline = await presenceService.SetOnlineAsync(userId, Context.ConnectionId);
            if (becameOnline)
            {
                await Clients.All.SendAsync("UserStatusChanged", userId.ToString(), "online");
            }

            // Join user-specific group to receive private messages
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var becameOffline = await presenceService.SetOfflineAsync(userId, Context.ConnectionId);
            if (becameOffline)
            {
                await Clients.All.SendAsync("UserStatusChanged", userId.ToString(), "offline");
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
    
    // Clients can call this when open an existing chat
    public async Task GetUserStatus(Guid userId)
    {
        var isOnline = await presenceService.IsOnlineAsync(userId);
        await Clients.Caller.SendAsync("ReceiveUserStatus", 
            userId.ToString(), isOnline ? "online" : "offline");
    }
    
    public async Task SendMessage(Guid chatId, string content)
    {
        var senderIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(senderIdClaim, out var senderId)) return;

        var chat = await chatService.GetAsync(chatId);
        if (chat == null) return;

        // Verify sender is part of the chat
        if (chat.User1Id != senderId && chat.User2Id != senderId) return;

        var recipientId = chat.User1Id == senderId ? chat.User2Id : chat.User1Id;

        // Store message
        var messageDto = await messageService.SendAsync(chatId, senderId, content);

        // Notify both sender and recipient
        await Clients.Group(senderId.ToString()).SendAsync("ReceiveMessage", messageDto);
        await Clients.Group(recipientId.ToString()).SendAsync("ReceiveMessage", messageDto);
    }
}