﻿namespace DiscordButBetter.Server.Contracts.Responses;

public class ConversationResponse
{
    public Guid ConversationId { get; set; }
    public string ConversationName { get; set; }
    public byte ConversationType { get; set; }
    public string ConversationPicture { get; set; }
}