using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Users;

public class FriendRemovedConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<FriendRemovedMessage>
{
    public async Task Consume(ConsumeContext<FriendRemovedMessage> context)
    {
        var request = context.Message;
        await hubContext.Clients.User(request.UserId.ToString()).FriendRemoved(request.FriendId);
        await hubContext.Clients.User(request.FriendId.ToString()).FriendRemoved(request.UserId);
    }
}