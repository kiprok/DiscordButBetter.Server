using DiscordButBetter.Server.Background;
using DiscordButBetter.Server.Consumers.Conversations;
using DiscordButBetter.Server.Consumers.FriendRequests;
using DiscordButBetter.Server.Consumers.Messages;
using DiscordButBetter.Server.Consumers.Users;
using MassTransit;

namespace DiscordButBetter.Server;

public static class RabbitExchangeConfigurator
{
    public static void ConfigureAllConsumers(this IBusRegistrationConfigurator cfg)
    {
        //conversation
        cfg.AddUniqueConsumer<AddedToConversationConsumer>();
        cfg.AddUniqueConsumer<RemovedFromConversationConsumer>();
        cfg.AddUniqueConsumer<NewConversationConsumer>();
        cfg.AddUniqueConsumer<ChangedConversationConsumer>();
        //friend requests
        cfg.AddUniqueConsumer<FriendRequestAcceptedConsumer>();
        cfg.AddUniqueConsumer<FriendRequestCanceledConsumer>();
        cfg.AddUniqueConsumer<FriendRequestDeclinedConsumer>();
        cfg.AddUniqueConsumer<FriendRequestSendConsumer>();
        //messages
        cfg.AddUniqueConsumer<SendChatMessageConsumer>();
        cfg.AddUniqueConsumer<EditChatMessageConsumer>();
        cfg.AddUniqueConsumer<DeleteChatMessageConsumer>();
        //users
        cfg.AddUniqueConsumer<FriendRemovedConsumer>();
        cfg.AddUniqueConsumer<UserInfoChangedConsumer>();
    }

    public static void AddUniqueConsumer<TConsumer>(this IBusRegistrationConfigurator cfg)
        where TConsumer : class, IConsumer
    {
        cfg.AddConsumer<TConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
    }
}