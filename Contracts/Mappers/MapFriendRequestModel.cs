using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database.Models;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapFriendRequestModel
{
    public static FriendRequestResponse ToResponse(this FriendRequestModel model)
    {
        return new FriendRequestResponse
        {
            RequestId = model.Id,
            SenderId = model.SenderId,
            ReceiverId = model.ReceiverId
        };
    }
    
    public static FriendRequestSendMessage ToSendMessage(this FriendRequestModel model)
    {
        return new FriendRequestSendMessage
        {
            RequestId = model.Id,
            SenderId = model.SenderId,
            ReceiverId = model.ReceiverId
        };
    }
    
    public static FriendRequestAcceptedMessage ToAcceptedMessage(this FriendRequestModel model)
    {
        return new FriendRequestAcceptedMessage
        {
            RequestId = model.Id,
            SenderId = model.SenderId,
            ReceiverId = model.ReceiverId
        };
    }
    
    public static FriendRequestDeclinedMessage ToDeclinedMessage(this FriendRequestModel model)
    {
        return new FriendRequestDeclinedMessage
        {
            RequestId = model.Id,
            SenderId = model.SenderId,
            ReceiverId = model.ReceiverId
        };
    }
    
    public static FriendRequestCanceledMessage ToCanceledMessage(this FriendRequestModel model)
    {
        return new FriendRequestCanceledMessage
        {
            RequestId = model.Id,
            SenderId = model.SenderId,
            ReceiverId = model.ReceiverId
        };
    }
}