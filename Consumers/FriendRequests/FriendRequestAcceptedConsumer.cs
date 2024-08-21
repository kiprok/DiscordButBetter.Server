using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.FriendRequests;
[UniqueEndpoint]
public class FriendRequestAcceptedConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<FriendRequestAcceptedMessage>
{
    public async Task Consume(ConsumeContext<FriendRequestAcceptedMessage> context)
    {
        var request = context.Message;
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestAccepted(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestAccepted(request);
    }
}