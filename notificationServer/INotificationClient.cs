using System.Text.Json.Nodes;
using DiscordButBetter.Server.Contracts.Responses;

namespace DiscordButBetter.Server.notificationServer;

public interface INotificationClient
{
    Task InitializedUser();
    
    Task ReceiveMessage(MessageResponse message);
}