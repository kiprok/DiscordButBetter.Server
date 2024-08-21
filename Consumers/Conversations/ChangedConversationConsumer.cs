using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Conversations;
[UniqueEndpoint]
public class ChangedConversationConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<ChangedConversationMessage>
{
    public async Task Consume(ConsumeContext<ChangedConversationMessage> context)
    {
        var conversation = context.Message;
        foreach (var participant in conversation.Participants)
            await hubContext.Clients.User(participant.ToString()).ConversationInfoChanged(conversation);
    }
}