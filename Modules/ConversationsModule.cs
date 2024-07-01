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
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/list/{id:guid}", (DbbContext db, Guid id) =>
        {
            var conversations = db.Users
                .Include(userModel => userModel.Conversations).FirstOrDefault(u => u.Id == id)
                ?.Conversations;

            if (conversations == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversations.Select(c => c.ToConversationResponse()));
        });
        
        app.MapGet("/list/visible/{id:guid}", (DbbContext db, Guid id) =>
        {
            var conversations = db.Users
                .Include(userModel => userModel.VisibleConversations).FirstOrDefault(u => u.Id == id)
                ?.VisibleConversations;

            if (conversations == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversations.Select(c => c.ToConversationResponse()));
        });
        
        app.MapGet("/{id:guid}", (DbbContext db, Guid id) =>
        {
            var conversation = db.Conversations.FirstOrDefault(c => c.Id == id);
            if (conversation == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(conversation.ToConversationResponse());
        });
        
        app.MapPost("/{id:guid}", async (DbbContext db,Guid id, [FromBody] CreateConversationRequest request) =>
        {
            if (request.ConversationType == 0)
            {
                var dm = db.Conversations
                    .FirstOrDefault(c => 
                        c.Participants.FirstOrDefault(u => u.Id == id) != null &&
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
                Participants = request.Participants.Select(p => db.Users.FirstOrDefault(u => u.Id == p)).ToList()!
            };
            conversation.Participants.Add(db.Users.FirstOrDefault(u => u.Id == id)!);
            
            db.Conversations.Add(conversation);
            await db.SaveChangesAsync();
            
            return Results.Ok(conversation.ToConversationResponse());
        });
    }
}