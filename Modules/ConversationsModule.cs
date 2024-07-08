using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
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

    private async Task<Results<Ok<ConversationResponse>, NotFound>> UpdateConversationById(DbbContext db,
        Guid conversationId,
        [FromBody] UpdateConversationRequest request)
    {
        var conversation = db.Conversations.Include(c => c.Participants)
            .FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null) return TypedResults.NotFound();

        if (request.ConversationName != null) conversation.ConversationName = request.ConversationName;
        if (request.ConversationPicture != null) conversation.ConversationPicture = request.ConversationPicture;
        if (request.ParticipantsToAdd != null)
            conversation.Participants.AddRange(db.Users.Where(u => request.ParticipantsToAdd.Contains(u.Id)));
        if (request.ParticipantsToRemove != null)
            conversation.Participants.RemoveAll(u => request.ParticipantsToRemove.Contains(u.Id));

        await db.SaveChangesAsync();
        return TypedResults.Ok(conversation.ToConversationResponse());
    }

    private async Task<Results<Ok, NotFound>> DeleteConversationById(DbbContext db, Guid conversationId,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var user = db.Users.Include(u => u.VisibleConversations)
            .FirstOrDefault(u => u.Id == userId);
        var conversation = db.Conversations.Include(c => c.Participants)
            .FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null || user == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(u => u.Id == userId) == null) return TypedResults.NotFound();

        if (conversation.ConversationType == 0)
        {
            user.VisibleConversations.Remove(conversation);
        }
        else
        {
            conversation.Participants.Remove(user);
            if (conversation.Participants.Count == 1) db.Conversations.Remove(conversation);
        }

        await db.SaveChangesAsync();
        return TypedResults.Ok();
    }

    private async Task<Ok<ConversationResponse>> CreateNewConversation(
        DbbContext db,
        [FromBody] CreateConversationRequest request,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        if (request.ConversationType == 0)
        {
            var dm = db.Conversations
                .Include(c => c.Participants)
                .Include(c => c.ParticipantsVisible)
                .FirstOrDefault(c =>
                    c.Participants.FirstOrDefault(u => u.Id == userId) != null &&
                    c.Participants.FirstOrDefault(u => u.Id == request.Participants[0]) != null);

            if (dm != null)
            {
                dm.ParticipantsVisible.Add(db.Users.FirstOrDefault(u => u.Id == userId)!);
                await db.SaveChangesAsync();
                return TypedResults.Ok(dm.ToConversationResponse());
            }
        }

        var participants = db.Users.Where(u => request.Participants.Contains(u.Id)).ToList();
        participants.Add(db.Users.FirstOrDefault(u => u.Id == userId)!);

        var conversation = new ConversationModel
        {
            Id = Guid.NewGuid(),
            ConversationName = request.ConversationName,
            ConversationType = request.ConversationType,
            ConversationPicture = "",
            Participants = participants,
            ParticipantsVisible = participants
        };

        db.Conversations.Add(conversation);
        await db.SaveChangesAsync();

        return TypedResults.Ok(conversation.ToConversationResponse());
    }

    private Results<Ok<ConversationResponse>, NotFound> GetConversationById(DbbContext db, Guid conversationId)
    {
        var conversation = db.Conversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null) return TypedResults.NotFound();

        return TypedResults.Ok(conversation.ToConversationResponse());
    }

    private async Task<Results<Ok, NotFound>> DeleteVisibleConversationById(DbbContext db, Guid conversationId,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var user = db.Users.Include(u => u.VisibleConversations)
            .ThenInclude(c => c.Participants)
            .FirstOrDefault(u => u.Id == userId);
        if (user == null) return TypedResults.NotFound();

        var conversation = user.VisibleConversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation == null) return TypedResults.NotFound();

        if (conversation.Participants.FirstOrDefault(u => u.Id == userId) == null)
            return TypedResults.NotFound();

        user.VisibleConversations.Remove(conversation);
        await db.SaveChangesAsync();
        return TypedResults.Ok();
    }

    private Results<Ok<List<Guid>>, NotFound> GetVisibleConversationsForUser(DbbContext db, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var conversations = db.Users
            .Include(userModel => userModel.VisibleConversations)
            .FirstOrDefault(u => u.Id == userId)
            ?.VisibleConversations.ToList();

        if (conversations == null) return TypedResults.NotFound();

        return TypedResults.Ok(conversations.Select(c => c.Id).ToList());
    }

    private Results<Ok<List<ConversationResponse>>, NotFound> GetConversationsForUser(DbbContext db,
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var conversations = db.Users.Include(u => u.Conversations)
            .ThenInclude(c => c.Participants)
            .FirstOrDefault(u => u.Id == userId)
            ?.Conversations;

        if (conversations == null) return TypedResults.NotFound();

        return TypedResults.Ok(conversations.Select(c => c.ToConversationResponse()).ToList());
    }
}