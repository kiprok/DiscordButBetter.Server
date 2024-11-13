using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        app.MapGet("/conversation/{conversationId:guid}/older/{messageTime:Datetime}", GetOlderMessages);

        app.MapGet("/conversation/{conversationId:guid}/newer/{messageTime:Datetime}", GetNewerMessages);

        app.MapGet("/conversation/{conversationId:guid}/point/{messageId:guid}", GetSurroundingMessages);

        app.MapGet("/{messageId:guid}", GetMessageById);

        app.MapPut("/", CreateNewMessage);

        app.MapDelete("/{messageId:guid}", DeleteMessageById);

        app.MapPatch("/{messageId:guid}", UpdateMessageById);

        app.MapGet("/conversation/{conversationId:guid}/search", SearchForMessages);
    }

    private async Task<Results<Ok<MessageResponse>, NotFound, BadRequest<string>>> UpdateMessageById(
        DbbContext db,
        Guid messageId,
        [FromBody] UpdateChatMessageRequest request,
        IBus bus)
    {
        var message = await db.Messages.FindAsync(messageId);
        if (message == null) return TypedResults.NotFound();


        if (request.Content.Length > 2000) return TypedResults.BadRequest("Message content is too long.");

        message.Content = request.Content;
        message.Metadata = request.Metadata.ToString();

        await db.SaveChangesAsync();

        var response = message.ToMessageResponse();

        await bus.Publish(message.ToEditChatMessageMessage());

        return TypedResults.Ok(response);
    }

    private async Task<Results<Ok, NotFound, UnauthorizedHttpResult>> DeleteMessageById(
        IMessageService messageService,
        Guid messageId,
        ClaimsPrincipal claim,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var message = await messageService.GetMessageById(messageId);

        if (message == null)
            return TypedResults.NotFound();
        if (message.SenderId != userId)
            return TypedResults.Unauthorized();

        if (!await messageService.DeleteMessageById(messageId))
            return TypedResults.NotFound();

        await bus.Publish(message.ToDeleteChatMessageMessage());
        return TypedResults.Ok();
    }

    private async Task<Results<Ok<MessageResponse>, NotFound, BadRequest, UnauthorizedHttpResult>> CreateNewMessage(
        IMessageService messageService,
        IConversationService conversationService,
        [FromBody] SendChatMessageRequest request,
        ClaimsPrincipal claim,
        IBus bus)
    {
        if (request.Content.Length > 2000) return TypedResults.BadRequest();
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.GetConversationById(request.ConversationId);

        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.Unauthorized();

        var message = request.ToChatMessageModel();
        message.SenderId = userId;
        var result = await messageService.CreateNewMessage(message);

        var response = result.ToMessageResponse();

        await bus.Publish(result.ToSendChatMessageMessage());

        return TypedResults.Ok(response);
    }

    private async Task<Results<Ok<MessageResponse>, NotFound>> GetMessageById(
        IMessageService messageService,
        IConversationService conversationService,
        ClaimsPrincipal claim,
        Guid messageId)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var message = await messageService.GetMessageById(messageId);
        if (message == null) return TypedResults.NotFound();

        var conversation = await conversationService.GetConversationById(message.ConversationId);

        if (conversation?.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(message.ToMessageResponse());
    }

    private async Task<Results<Ok<List<MessageResponse>>, NotFound, UnauthorizedHttpResult>> GetNewerMessages(
        IConversationService conversationService,
        IMessageService messageService,
        ClaimsPrincipal claim,
        Guid conversationId,
        DateTime messageTime)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.GetConversationById(conversationId);

        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.Unauthorized();

        var messages = await messageService.GetNewerMessages(conversationId, 50, messageTime);

        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }

    private async Task<Results<Ok<List<MessageResponse>>, NotFound, UnauthorizedHttpResult>> GetOlderMessages(
        IConversationService conversationService,
        IMessageService messageService,
        ClaimsPrincipal claim,
        Guid conversationId,
        DateTime messageTime)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.GetConversationById(conversationId);

        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.Unauthorized();

        var messages = await messageService.GetOlderMessages(conversationId, 50, messageTime);

        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }

    private async Task<Results<Ok<List<MessageResponse>>, NotFound, UnauthorizedHttpResult>>
        GetMessagesFromConversation(
            IConversationService conversationService,
            IMessageService messageService,
            ClaimsPrincipal claim,
            Guid conversationId)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.GetConversationById(conversationId);

        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.Unauthorized();

        var messages = await messageService.GetMessagesFromConversation(conversationId);

        return TypedResults.Ok(messages.Select(m => m.ToMessageResponse()).ToList());
    }

    private async Task<Results<Ok<List<MessageResponse>>, NotFound, UnauthorizedHttpResult>> GetSurroundingMessages(
        IConversationService conversationService,
        IMessageService messageService,
        ClaimsPrincipal claim,
        Guid conversationId,
        Guid messageId)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.GetConversationById(conversationId);

        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(x => x.Id == userId) == null)
            return TypedResults.Unauthorized();

        var targetMessage = await messageService.GetMessageById(messageId);
        if (targetMessage == null) return TypedResults.NotFound();

        var messagesAbove = await messageService.GetOlderMessages(conversationId, 25, targetMessage.SentAt);

        var messagesBelow = await messageService.GetNewerMessages(conversationId, 25, targetMessage.SentAt);

        var combinedMessages = messagesAbove
            .Concat(new[] { targetMessage })
            .Concat(messagesBelow)
            .OrderByDescending(m => m.SentAt);

        return TypedResults.Ok(combinedMessages.Select(m => m.ToMessageResponse()).ToList());
    }

    private Ok<MessageSearchResponse> SearchForMessages(
        DbbContext db,
        Guid conversationId,
        [FromQuery] string query,
        [FromQuery] int page = 1)
    {
        var messages = db.Messages
            .Where(m => m.ConversationId == conversationId)
            .Where(m => m.Content.Contains(query))
            .OrderByDescending(m => m.SentAt)
            .ToList();


        return TypedResults.Ok(new MessageSearchResponse
        {
            TotalCount = messages.Count,
            Messages = messages
                .Skip((page - 1) * 25)
                .Take(25)
                .Select(m => m.ToMessageResponse()).ToList()
        });
    }
}