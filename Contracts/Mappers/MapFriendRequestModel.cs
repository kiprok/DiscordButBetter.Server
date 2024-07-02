﻿using DiscordButBetter.Server.Contracts.Requests;
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
            SenderId = model.SenderId
        };
    }
    
    public static FriendRequestModel ToModel(this FriendRequestRequest request)
    {
        return new FriendRequestModel
        {
            SenderId = request.UserId,
            ReceiverId = request.RequestId ?? Guid.Empty
        };
    }
}