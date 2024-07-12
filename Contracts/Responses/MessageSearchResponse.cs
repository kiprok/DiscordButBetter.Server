namespace DiscordButBetter.Server.Contracts.Responses;

public class MessageSearchResponse
{
    public int TotalCount { get; set; }
    public List<MessageResponse> Messages { get; set; }
}