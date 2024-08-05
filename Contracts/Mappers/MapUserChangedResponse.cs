using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.Contracts.Responses;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapUserChangedResponse
{
    public static UserUpdateResponse ToUserUpdateResponse(this UserInfoChangedMessage message)
    {
        return new UserUpdateResponse
        {
            UserId = message.UserId,
            ProfilePicture = message.ProfilePicture,
            Status = message.Status,
            StatusMessage = message.StatusMessage,
            Biography = message.Biography
        };
    }

    public static UserInfoChangedMessage ToUserInfoChangedMessage(this UserUpdateResponse response)
    {
        return new UserInfoChangedMessage
        {
            UserId = response.UserId,
            ProfilePicture = response.ProfilePicture,
            Status = response.Status,
            StatusMessage = response.StatusMessage,
            Biography = response.Biography
        };
    }
}