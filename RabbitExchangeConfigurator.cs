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
        #region Conversations

        cfg.AddConsumer<AddedToConversationConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<RemovedFromConversationConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<NewConversationConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<ChangedConversationConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });

        #endregion

        #region friendRequests

        cfg.AddConsumer<FriendRequestAcceptedConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<FriendRequestCanceledConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<FriendRequestDeclinedConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<FriendRequestSendConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        #endregion

        #region messages

        cfg.AddConsumer<SendChatMessageConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<EditChatMessageConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<DeleteChatMessageConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });

        #endregion

        #region users

        cfg.AddConsumer<FriendRemovedConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });
        
        cfg.AddConsumer<UserInfoChangedConsumer>()
            .Endpoint(e =>
            {
                e.Temporary = true;
                e.InstanceId = HeartBeatService.ServiceId.ToString();
            });

        #endregion
    }
}