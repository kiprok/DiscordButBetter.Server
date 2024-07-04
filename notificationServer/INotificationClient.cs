using System.Text.Json.Nodes;
using DiscordButBetter.Server.Contracts.Responses;

namespace DiscordButBetter.Server.notificationServer;

public interface INotificationClient
{
    Task ReceiveNotification(string notification, UserResponse user);
}