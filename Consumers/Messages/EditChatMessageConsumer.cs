using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Messages;
[UniqueEndpoint]
public class EditChatMessageConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<EditChatMessageMessage>
{
    public async Task Consume(ConsumeContext<EditChatMessageMessage> context)
    {
        var message = context.Message;
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageEdited(message);
    }
}