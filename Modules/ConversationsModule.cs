using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Modules;

public class ConversationsModule : CarterModule
{
    public ConversationsModule() : base("/api/conversations")
    {
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var conversations = db.Users
                .Include(u => u.Conversations)
                .ThenInclude(c => c.Participants)
                .FirstOrDefault(u => u.Id == userId)
                ?.Conversations;

            if (conversations == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversations.Select(c => c.ToConversationResponse()));
        });
        
        app.MapGet("/visible", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var conversations = db.Users
                .Include(userModel => userModel.VisibleConversations)
                .FirstOrDefault(u => u.Id == userId)
                ?.VisibleConversations.ToList();

            if (conversations == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversations.Select(c => c.Id));
        });
        
        app.MapDelete("/visible/{conversationId:Guid}", async (
            DbbContext db,
            Guid conversationId,
            ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var user = db.Users
                .Include(u => u.VisibleConversations)
                .ThenInclude(c => c.Participants)
                .FirstOrDefault(u => u.Id == userId);
            if(user == null)
                return Results.NotFound();
            
            var conversation = user.VisibleConversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null)
                return Results.NotFound();

            if (conversation.Participants.FirstOrDefault(u => u.Id == userId) == null)
                return Results.NotFound();
            
            user.VisibleConversations.Remove(conversation);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
        
        app.MapGet("/{conversationId:guid}", (DbbContext db, Guid conversationId) =>
        {
            var conversation = db.Conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversation.ToConversationResponse());
        });
        
        app.MapPut("/", 
            async (DbbContext db, [FromBody] CreateConversationRequest request, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            if (request.ConversationType == 0)
            {
                var dm = db.Conversations
                    .FirstOrDefault(c => 
                        c.Participants.FirstOrDefault(u => u.Id == userId) != null &&
                        c.Participants.FirstOrDefault(u => u.Id == request.Participants[0]) != null);
                
                if (dm != null)
                    return Results.Ok(dm.ToConversationResponse());
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
            
            return Results.Ok(conversation.ToConversationResponse());
        });

        app.MapDelete("/{conversationId:Guid}", async (
            DbbContext db,
            Guid conversationId,
            ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var user = db.Users
                .Include(u => u.VisibleConversations)
                .FirstOrDefault(u => u.Id == userId);
            var conversation = db.Conversations
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null || user == null)
            {
                return Results.NotFound();
            }

            if (conversation.Participants.FirstOrDefault(u => u.Id == userId) == null)
            {
                return Results.NotFound();
            }
            
            if(conversation.ConversationType == 0)
            {
                user.VisibleConversations.Remove(conversation);
            }
            else
            {
                conversation.Participants.Remove(user);
                if(conversation.Participants.Count == 1)
                    db.Conversations.Remove(conversation);
            }
            
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapPatch("/{conversationId:Guid}", 
            async (DbbContext db,Guid conversationId, [FromBody]UpdateConversationRequest request) =>
        {
            var conversation = db.Conversations
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null)
            {
                return Results.NotFound();
            }

            if (request.ConversationName != null)
                conversation.ConversationName = request.ConversationName;
            if (request.ConversationPicture != null)
                conversation.ConversationPicture = request.ConversationPicture;
            if(request.ParticipantsToAdd != null)
                conversation.Participants.AddRange(db.Users.Where(u => request.ParticipantsToAdd.Contains(u.Id)));
            if(request.ParticipantsToRemove != null)
                conversation.Participants.RemoveAll(u => request.ParticipantsToRemove.Contains(u.Id));
            
            await db.SaveChangesAsync();
            return Results.Ok(conversation.ToConversationResponse());
        });
        
    }
}