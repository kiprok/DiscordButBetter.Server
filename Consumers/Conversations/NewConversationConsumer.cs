using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Conversations;
[UniqueEndpoint]
public class NewConversationConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<NewConversationMessage>
{
    public async Task Consume(ConsumeContext<NewConversationMessage> context)
    {
        var conversation = context.Message;
        foreach (var participant in conversation.Participants)
        {
            await NotificationHub.AddToGroupAsync(hubContext, participant, conversation.ConversationId);
            await hubContext.Clients.User(participant.ToString()).CreatedNewConversation(conversation);
        }
    }
}