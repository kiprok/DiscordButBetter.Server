using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Users;
[UniqueEndpoint]
public class UserInfoChangedConsumer(
    IHubContext<NotificationHub, INotificationClient> hubContext,
    ILogger<UserInfoChangedConsumer> logger)
    : IConsumer<UserInfoChangedMessage>
{
    public async Task Consume(ConsumeContext<UserInfoChangedMessage> context)
    {
        var user = context.Message;
        logger.LogInformation("User {UserId} info changed", user.UserId);
        await hubContext.Clients.Group(user.UserId.ToString()).UserInfoChanged(user);
    }
}