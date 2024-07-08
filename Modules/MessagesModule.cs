using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiscordButBetter.Server.Modules;

public class MessagesModule : CarterModule
{
    public MessagesModule() : base("/api/messages")
    {
        RequireAuthorization();
        IncludeInOpenApi();
        WithTags("Messages");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/conversation/{conversationId:guid}", GetMessagesFromConversation);

        app.MapGet("/conversation/{conversationId:guid}/older/{messageTime:Datetime}",
            GetOlderMesssagesFromConversation);

        app.MapGet("/conversation/{conversationId:guid}/newer/{messageTime:Datetime}",
            GetNewerMessagesFromConversation);

        app.MapGet("/{messageId:guid}", GetMessageById);

        app.MapPut("/", CreateNewMessage);

        app.MapDelete("/{messageId:guid}", DeleteMessageById);

        app.MapPatch("/{messageId:guid}", UpdateMessageById);
    }

    private async Task<Results<Ok<MessageResponse>, NotFound>> UpdateMessageById(DbbContext db, Guid messageId,
        [FromBody] UpdateChatMessageRequest request)
    {
        var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null) return TypedResults.NotFound();

        message.Content = request.Content;
        message.Metadata = request.Metadata.ToString();

        await db.SaveChangesAsync();

        return TypedResults.Ok(message.ToMessageResponse());
    }

    private async Task<Results<Ok, NotFound>> DeleteMessageById(DbbContext db, Guid messageId)
    {
        var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null) return TypedResults.NotFound();

        db.Messages.Remove(message);
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

    private async Task<Ok<MessageResponse>> CreateNewMessage(DbbContext db, [FromBody] SendChatMessageRequest request,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var message = request.ToChatMessageModel();
        message.Id = Guid.NewGuid();
        message.SenderId = userId;

        db.Messages.Add(message);
        await db.SaveChangesAsync();

        return TypedResults.Ok(message.ToMessageResponse());
    }

    private Results<Ok<MessageResponse>, NotFound> GetMessageById(DbbContext db, Guid messageId)
    {
        var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null) return TypedResults.NotFound();

        return TypedResults.Ok(message.ToMessageResponse());
    }

    private Ok<List<MessageResponse>> GetNewerMessagesFromConversation(DbbContext db, Guid conversationId,
        DateTime messageTime)
    {
        var messages = db.Messages.Where(m => m.ConversationId == conversationId && m.SentAt > messageTime)
            .OrderByDescending(m => m.SentAt)
            .Take(50);
        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }

    private Ok<List<MessageResponse>> GetOlderMesssagesFromConversation(DbbContext db, Guid conversationId,
        DateTime messageTime)
    {
        var messages = db.Messages.Where(m => m.ConversationId == conversationId && m.SentAt < messageTime)
            .OrderByDescending(m => m.SentAt)
            .Take(50);
        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }

    private Ok<List<MessageResponse>> GetMessagesFromConversation(DbbContext db, Guid conversationId)
    {
        var messages = db.Messages.Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(50);
        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }
}