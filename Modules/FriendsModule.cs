using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Modules;

public class FriendsModule : CarterModule
{
    public FriendsModule() : base("/api/users/friends")
    {
        RequireAuthorization();
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.Friends.Select(f => f.ToUserResponse()));
        });

        app.MapDelete("/{friendId:guid}", (DbbContext db, ClaimsPrincipal claim, Guid friendId) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Results.NotFound();
            }
            var friend = user.Friends.FirstOrDefault(f => f.Id == friendId);
            if (friend == null)
            {
                return Results.NotFound();
            }
            user.Friends.Remove(friend);
            db.SaveChanges();
            return Results.Ok();
        });
        
        app.MapGet("/requests", (DbbContext db, ClaimsPrincipal claim) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            
            var requests = db.FriendRequests.Where(r => r.ReceiverId == userId || r.SenderId == userId).ToList();
            return Results.Ok(requests.Select(r => r.ToResponse()));
        });

        app.MapPost("/requests", async (
            [FromBody] FriendRequestRequest friendRequest,
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