using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Messages;
[UniqueEndpoint]
public class DeleteChatMessageConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<DeleteChatMessageMessage>
{
    public async Task Consume(ConsumeContext<DeleteChatMessageMessage> context)
    {
        var message = context.Message;
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageDeleted(message.MessageId);
    }
}