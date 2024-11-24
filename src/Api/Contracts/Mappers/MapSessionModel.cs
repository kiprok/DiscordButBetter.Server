using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database.Models;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapSessionModel
{
    public static SessionResponse ToSessionResponse(this SessionModel session)
    {
        return new SessionResponse
        {
            Token = session.token,
            UserId = session.userId.ToString()
        };
    }
}