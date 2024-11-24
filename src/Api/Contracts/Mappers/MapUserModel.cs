using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database.Models;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapUserModel
{
    public static UserResponse ToUserResponse(this UserModel user)
    {
        return new UserResponse
        {
            UserId = user.Id,
            Username = user.Username,
            ProfilePicture = user.ProfilePicture,
            Status = user.Status,
            StatusMessage = user.StatusMessage,
            Biography = user.Biography
        };
    }
}