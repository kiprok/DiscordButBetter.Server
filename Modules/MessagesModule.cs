using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscordButBetter.Server.Modules;

public class MessagesModule : CarterModule
{
    public MessagesModule() : base("/api/messages")
    {
        RequireAuthorization();
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/conversation/{conversationId:guid}", 
            (DbbContext db, Guid conversationId) =>
        {
            var messages = db.Messages.
                Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt).Take(50);
            return Results.Ok(messages.Select(m => m.ToMessageResponse()));
        });
        
        app.MapGet("/conversation/{conversationId:guid}/older/{messageTime:Datetime}", 
            (DbbContext db, Guid conversationId, DateTime messageTime) =>
        {
            var messages = db.Messages
                .Where(m => m.ConversationId == conversationId && m.SentAt < messageTime)
                .OrderByDescending(m => m.SentAt)
                .Take(50);
            return Results.Ok(messages.Select(m => m.ToMessageResponse()));
        });
        
        app.MapGet("/conversation/{conversationId:guid}/newer/{messageTime:Datetime}", 
            (DbbContext db, Guid conversationId, DateTime messageTime) =>
            {
                var messages = db.Messages
                    .Where(m => m.ConversationId == conversationId && m.SentAt > messageTime)
                    .OrderByDescending(m => m.SentAt)
                    .Take(50);
                return Results.Ok(messages.Select(m => m.ToMessageResponse()));
            });
        
        app.MapGet("/{messageId:guid}", 
            (DbbContext db, Guid messageId) =>
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(message.ToMessageResponse());
        });
        
        app.MapPost("/", 
            async (DbbContext db,[FromBody] SendMessageRequest request) =>
        {
            var message = request.ToChatMessageModel();
            message.Id = Guid.NewGuid();
            
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            
            return Results.Ok(message.ToMessageResponse());
        });

    }
}