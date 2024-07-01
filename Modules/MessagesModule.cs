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
        
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/conversation/{id:guid}", (DbbContext db, Guid id) =>
        {
            var messages = db.Messages.Where(m => m.ConversationId == id);
            return Results.Ok(messages.Select(m => m.ToMessageResponse()));
        });
        
        app.MapGet("/{id:guid}", (DbbContext db, Guid id) =>
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(message.ToMessageResponse());
        });
        
        app.MapPost("/", async (DbbContext db,[FromBody] SendMessageRequest request) =>
        {
            var message = request.ToChatMessageModel();
            message.Id = Guid.NewGuid();
            
            
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            
            return Results.Ok(message.ToMessageResponse());
        });

    }
}