using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.FriendRequests;
[UniqueEndpoint]
public class FriendRequestCanceledConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<FriendRequestCanceledMessage>
{
    public async Task Consume(ConsumeContext<FriendRequestCanceledMessage> context)
    {
        var request = context.Message;
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestCanceled(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestCanceled(request);
    }
}