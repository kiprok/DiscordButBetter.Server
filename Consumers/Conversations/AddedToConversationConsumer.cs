using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Conversations;
[UniqueEndpoint]
public class AddedToConversationConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<AddedToConversationMessage>
{
    public async Task Consume(ConsumeContext<AddedToConversationMessage> context)
    {
        var conversation = context.Message;
        await NotificationHub.AddToGroupAsync(hubContext, conversation.UserId, conversation.ConversationId);
        foreach (var participant in conversation.Participants)
            await hubContext.Clients.User(participant.ToString())
                .AddedToConversation(conversation);
    }
}