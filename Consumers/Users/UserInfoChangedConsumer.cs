using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Users;

public class UserInfoChangedConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<UserInfoChangedMessage>
{
    public async Task Consume(ConsumeContext<UserInfoChangedMessage> context)
    {
        var user = context.Message;
        await hubContext.Clients.Group(user.UserId.ToString()).UserInfoChanged(user);
    }
}