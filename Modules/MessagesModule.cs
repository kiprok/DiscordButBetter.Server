﻿using System.Security.Claims;
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
        
        app.MapPut("/", 
            async (DbbContext db,[FromBody] SendChatMessageRequest request, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var message = request.ToChatMessageModel();
            message.Id = Guid.NewGuid();
            message.SenderId = userId;
            
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            
            return Results.Ok(message.ToMessageResponse());
        });
        
        app.MapDelete("/{messageId:guid}", 
            async (DbbContext db, Guid messageId) =>
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return Results.NotFound();
            }
            
            db.Messages.Remove(message);
            await db.SaveChangesAsync();
            
            return Results.Ok();
        });
        
        app.MapPatch("/{messageId:guid}", 
            async (DbbContext db, Guid messageId, [FromBody] UpdateChatMessageRequest request) =>
        {
            var message = db.Messages.FirstOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return Results.NotFound();
            }
            
            if (request.Content != null) message.Content = request.Content;
            if (request.Metadata != null) message.Metadata = request.Metadata.ToString();
            
            await db.SaveChangesAsync();
            
            return Results.Ok(message.ToMessageResponse());
        });
    }
}