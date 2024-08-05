using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Messages;

public class SendChatMessageConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<SendChatMessageMessage>
{
    public async Task Consume(ConsumeContext<SendChatMessageMessage> context)
    {
        var message = context.Message;
        await hubContext.Clients.Groups(message.ConversationId.ToString()).NewMessage(message);
    }
}