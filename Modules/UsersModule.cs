using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Modules;

public class UsersModule : CarterModule
{
    public UsersModule() :base("/api/users")
    {
        RequireAuthorization();
    }
    
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/",  (DbbContext db) =>
        { 
            var users = db.Users.ToList();
            return Results.Ok(users.Select(u => u.ToUserResponse()));
        });
        
        app.MapGet("/{id:guid}", (DbbContext db, Guid id) =>
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.ToUserResponse());
        });
        
        app.MapGet("/friends/", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.Friends.Select(f => f.ToUserResponse()));
        });
        
        app.MapGet("/friends/requests", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var requests = db.FriendRequests.Where(r => r.ReceiverId == userId || r.SenderId == userId).ToList();
            return Results.Ok(requests.Select(r => r.ToResponse()));
        });

        app.MapPost("/friends/requests", async ([FromBody] FriendRequestRequest friendRequest,
            DbbContext db,
            ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var targetUser = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == friendRequest.UserId);
            var req = db.FriendRequests.FirstOrDefault(r => r.Id == friendRequest.RequestId);

            if (targetUser == null)
            {
                return Results.NotFound();
            }

            switch (friendRequest.Type)
            {
                case ReqeustType.Send:
                    var request = new FriendRequestModel
                    {
                        SenderId = userId,
                        ReceiverId = targetUser.Id
                    };
                    db.FriendRequests.Add(request);
                    await db.SaveChangesAsync();
                    return Results.Ok();
                case ReqeustType.Accept:
                    if (req == null)
                    {
                        return Results.NotFound();
                    }
                    var currentUser = db.Users.Include(u => u.Friends).First(u => u.Id == userId);
                    currentUser.Friends.Add(targetUser);
                    targetUser.Friends.Add(currentUser);
                    db.FriendRequests.Remove(req);
                    await db.SaveChangesAsync();
                    return Results.Ok();
                case ReqeustType.Decline:
                case ReqeustType.Cancel:
                    if (req == null)
                    {
                        return Results.NotFound();
                    }
                    db.FriendRequests.Remove(req);
                    await db.SaveChangesAsync();
                    return Results.Ok();
                default:
                    return Results.BadRequest();
            }
        });

    }
}