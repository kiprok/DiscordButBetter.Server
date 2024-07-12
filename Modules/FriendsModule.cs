using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Modules;

public class FriendsModule : CarterModule
{
    public FriendsModule() : base("/api/users/friends")
    {
        RequireAuthorization();
        IncludeInOpenApi();
        WithTags("Friends");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetFriendsForUser);

        app.MapDelete("/{friendId:guid}", RemoveFriendForUser);

        app.MapGet("/requests", GetFriendRequestsForUser);

        app.MapPost("/requests", HandleFriendRequestForUser);
    }

    private async Task<Results<Ok<FriendRequestResponse>, Ok, NotFound, BadRequest>> HandleFriendRequestForUser(
        [FromBody] FriendRequestRequest friendRequest, 
        DbbContext db, 
        ClaimsPrincipal claim,
        INotificationService notificationService)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var targetUser = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == friendRequest.UserId);
        var req = db.FriendRequests.FirstOrDefault(r => r.Id == friendRequest.RequestId);

        if (targetUser == null) return TypedResults.NotFound();

        switch (friendRequest.Type)
        {
            case RequestType.Send:
                var request = new FriendRequestModel { SenderId = userId, ReceiverId = targetUser.Id };
                db.FriendRequests.Add(request);
                await db.SaveChangesAsync();
                await notificationService.SendFriendRequestNotification(request.ToResponse());
                return TypedResults.Ok(request.ToResponse());
            case RequestType.Accept:
                if (req == null) return TypedResults.NotFound();

                var currentUser = db.Users.Include(u => u.Friends).First(u => u.Id == userId);
                currentUser.Friends.Add(targetUser);
                targetUser.Friends.Add(currentUser);
                db.FriendRequests.Remove(req);
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            case RequestType.Decline:
            case RequestType.Cancel:
                if (req == null) return TypedResults.NotFound();

                db.FriendRequests.Remove(req);
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            default:
                return TypedResults.BadRequest();
        }
    }

    private Ok<List<FriendRequestResponse>> GetFriendRequestsForUser(DbbContext db, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var requests = db.FriendRequests.Where(r => r.ReceiverId == userId || r.SenderId == userId).ToList();
        return TypedResults.Ok(requests.Select(r => r.ToResponse()).ToList());
    }

    private Results<Ok, NotFound> RemoveFriendForUser(DbbContext db, ClaimsPrincipal claim, Guid friendId)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
        if (user == null) return TypedResults.NotFound();

        var friend = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == friendId);
        if (friend == null) return TypedResults.NotFound();

        user.Friends.Remove(friend);
        friend.Friends.Remove(user);
        db.SaveChanges();
        return TypedResults.Ok();
    }

    private Results<Ok<List<UserResponse>>, NotFound> GetFriendsForUser(DbbContext db, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == userId);
        if (user == null) return TypedResults.NotFound();

        return TypedResults.Ok(user.Friends.Select(f => f.ToUserResponse()).ToList());
    }
}