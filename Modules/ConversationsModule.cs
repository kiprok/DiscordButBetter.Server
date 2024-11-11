using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Messages;
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

public class ConversationsModule : CarterModule
{
    public ConversationsModule() : base("/api/conversations")
    {
        RequireAuthorization();
        IncludeInOpenApi();
        WithTags("Conversations");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetConversationsForUser);

        app.MapGet("/visible", GetVisibleConversationsForUser);

        app.MapDelete("/visible/{conversationId:Guid}", DeleteVisibleConversationById);

        app.MapGet("/{conversationId:guid}", GetConversationById);

        app.MapPut("/", CreateNewConversation);

        app.MapDelete("/{conversationId:Guid}", DeleteConversationById);

        app.MapPatch("/{conversationId:Guid}", UpdateConversationById);
    }

    private async Task<Results<Ok<ConversationUpdateResponse>, NotFound>> UpdateConversationById(
        IConversationService conversationService,
        Guid conversationId,
        [FromBody] UpdateConversationRequest request,
        ClaimsPrincipal claim,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.UpdateConversationById(userId, conversationId, request);

        if (conversation is null)
            return TypedResults.NotFound();

        await bus.Publish(
            conversation.ToChangedConversationMessage(request.ParticipantsToAdd, request.ParticipantsToRemove));

        return TypedResults.Ok(request.ToConversationUpdateResponse(conversation.Id));
    }

    private async Task<Results<Ok, NotFound>> DeleteConversationById(
        IConversationService conversationService,
        Guid conversationId,
        ClaimsPrincipal claim,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversation = await conversationService.DeleteConversationById(userId, conversationId);

        if (conversation is null)
            return TypedResults.NotFound();

        if (conversation.ConversationType == 0)
            return TypedResults.Ok();

        var message = new ChangedConversationMessage
        {
            ConversationId = conversationId,
            OwnerId = conversation.OwnerId != userId ? conversation.OwnerId : null,
            Participants = conversation.Participants.Select(u => u.Id).ToList(),
            ParticipantsToRemove = new List<Guid> { userId }
        };

        await bus.Publish(message);
        return TypedResults.Ok();
    }

    private async Task<Results<Ok<ConversationResponse>, BadRequest>> CreateNewConversation(
        IConversationService conversationService,
        [FromBody] CreateConversationRequest request,
        ClaimsPrincipal claim,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        if (request.ConversationType == 0)
        {
            var dm = await conversationService.GetPrivateConversation(userId, request.Participants.First());

            if (dm is not null)
            {
                if (await conversationService.AddUserToVisibleConversation(userId, dm.Id))
                    return TypedResults.Ok(dm.ToConversationResponse());
                return TypedResults.BadRequest();
            }
        }

        var conversation = await conversationService.CreateNewConversation(userId, request);

        if (conversation is null)
            return TypedResults.BadRequest();

        var response = conversation.ToConversationResponse();
        await bus.Publish(conversation.ToNewConversationMessage());

        return TypedResults.Ok(response);
    }

    private async Task<Results<Ok<ConversationResponse>, NotFound>> GetConversationById(
        IConversationService conversationService,
        Guid conversationId)
    {
        var conversation = await conversationService.GetConversationById(conversationId);
        if (conversation == null) return TypedResults.NotFound();

        return TypedResults.Ok(conversation.ToConversationResponse());
    }

    private async Task<Results<Ok, NotFound>> DeleteVisibleConversationById(
        IConversationService conversationService,
        Guid conversationId,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        if (await conversationService.DeleteVisibleConversationById(userId, conversationId))
            return TypedResults.Ok();
        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<List<Guid>>, NotFound>> GetVisibleConversationsForUser(
        IConversationService conversationService,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversations = await conversationService.GetVisibleConversationsForUser(userId);

        if (conversations is null) return TypedResults.NotFound();

        return TypedResults.Ok(conversations.Select(c => c.Id).ToList());
    }

    private async Task<Results<Ok<List<ConversationResponse>>, NotFound>> GetConversationsForUser(
        IConversationService conversationService,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var conversations = await conversationService.GetConversationsForUser(userId);

        if (conversations is null) return TypedResults.NotFound();

        return TypedResults.Ok(conversations.Select(c => c.ToConversationResponse()).ToList());
    }
}