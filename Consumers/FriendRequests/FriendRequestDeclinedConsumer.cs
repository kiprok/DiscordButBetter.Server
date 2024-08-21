using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.FriendRequests;
[UniqueEndpoint]
public class FriendRequestDeclinedConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<FriendRequestDeclinedMessage>
{
    public async Task Consume(ConsumeContext<FriendRequestDeclinedMessage> context)
    {
        var request = context.Message;
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestDeclined(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestDeclined(request);
    }
}