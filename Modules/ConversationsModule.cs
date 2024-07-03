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
                .Include(userModel => userModel.Conversations).FirstOrDefault(u => u.Id == userId)
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
                .Include(userModel => userModel.VisibleConversations).FirstOrDefault(u => u.Id == userId)
                ?.VisibleConversations;

            if (conversations == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversations.Select(c => c.ToConversationResponse()));
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
        
        app.MapPut("/{conversationId:guid}", async (DbbContext db,Guid conversationId, [FromBody] CreateConversationRequest request) =>
        {
            if (request.ConversationType == 0)
            {
                var dm = db.Conversations
                    .FirstOrDefault(c => 
                        c.Participants.FirstOrDefault(u => u.Id == conversationId) != null &&
                        c.Participants.FirstOrDefault(u => u.Id == request.Participants[0]) != null);
                
                if (dm != null)
                    return Results.Ok(dm.ToConversationResponse());
            }
            
            var conversation = new ConversationModel
            {
                Id = Guid.NewGuid(),
                ConversationName = request.ConversationName,
                ConversationType = request.ConversationType,
                ConversationPicture = "",
                Participants = db.Users.Where(u => request.Participants.Contains(u.Id)).ToList()
            };
            conversation.Participants.Add(db.Users.FirstOrDefault(u => u.Id == conversationId)!);
            
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
                db.Conversations.Remove(conversation);
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
    }
}