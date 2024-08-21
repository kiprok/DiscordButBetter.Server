using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.FriendRequests;
[UniqueEndpoint]
public class FriendRequestSendConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<FriendRequestSendMessage>
{
    public async Task Consume(ConsumeContext<FriendRequestSendMessage> context)
    {
        var request = context.Message;
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequest(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequest(request);
    }
}