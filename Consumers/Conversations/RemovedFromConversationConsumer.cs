using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.notificationServer;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Consumers.Conversations;

public class RemovedFromConversationConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    : IConsumer<RemovedFromConversationMessage>
{
    public async Task Consume(ConsumeContext<RemovedFromConversationMessage> context)
    {
        var conversation = context.Message;
        await NotificationHub.RemoveFromGroupAsync(hubContext, conversation.UserId, conversation.ConversationId);
        await hubContext.Clients.User(conversation.UserId.ToString())
            .RemovedFromConversation(conversation.ConversationId, conversation.UserId);
        foreach (var participant in conversation.Participants)
            await hubContext.Clients.User(participant.ToString())
                .RemovedFromConversation(conversation.ConversationId, conversation.UserId);
    }
}