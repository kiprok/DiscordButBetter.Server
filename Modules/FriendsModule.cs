using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using MassTransit;
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

        app.MapPost("/requests/Send", SendFriendRequest);
        app.MapPost("/requests/Accept", AcceptFriendRequest);
        app.MapPost("/requests/Decline", DeclineFriendRequest);
        app.MapPost("/requests/Cancel", CancelFriendRequest);
    }

    private async Task<Results<Ok<FriendRequestResponse>, Ok, NotFound, BadRequest>> SendFriendRequest(
        [FromBody] FriendRequestRequest friendRequest,
        ClaimsPrincipal claim,
        IFriendService friendService,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        
        var request = await friendService.SendFriendRequest(userId, friendRequest.UserId);
        if (request == null) 
            return TypedResults.BadRequest();
        
        await bus.Publish(request.ToSendMessage());
        return TypedResults.Ok(request.ToResponse());
    }
    
    private async Task<Results<Ok<FriendRequestResponse>, Ok, NotFound, BadRequest>> AcceptFriendRequest(
        [FromBody] FriendRequestRequest friendRequest,
        ClaimsPrincipal claim,
        IFriendService friendService,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        
        var request = await friendService.AcceptFriendRequest(userId, (Guid)friendRequest.RequestId!);
        if (request == null) 
            return TypedResults.BadRequest();
        
        await bus.Publish(request.ToAcceptedMessage());
        return TypedResults.Ok();
    }
    
    private async Task<Results<Ok<FriendRequestResponse>, Ok, NotFound, BadRequest>> DeclineFriendRequest(
        [FromBody] FriendRequestRequest friendRequest,
        ClaimsPrincipal claim,
        IFriendService friendService,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var request = await friendService.DeclineFriendRequest(userId, (Guid)friendRequest.RequestId!);
        if (request == null) 
            return TypedResults.BadRequest();
        
        await bus.Publish(request.ToDeclinedMessage());
        
        return TypedResults.Ok();
    }
    
    private async Task<Results<Ok<FriendRequestResponse>, Ok, NotFound, BadRequest>> CancelFriendRequest(
        [FromBody] FriendRequestRequest friendRequest,
        ClaimsPrincipal claim,
        IFriendService friendService,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        
        var request = await friendService.CancelFriendRequest(userId, (Guid)friendRequest.RequestId!);
        if (request == null) 
            return TypedResults.BadRequest();
        
        await bus.Publish(request.ToCanceledMessage());
        
        return TypedResults.Ok();
    }

    private async Task<Results<Ok<List<FriendRequestResponse>>,NotFound>> GetFriendRequestsForUser(
        IFriendService friendService, 
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var requests = await friendService.GetFriendRequestsForUser(userId);
        if (requests is null) 
            return TypedResults.NotFound();
        return TypedResults.Ok(requests.Select(r => r.ToResponse()).ToList());
    }

    private async Task<Results<Ok, NotFound>> RemoveFriendForUser(
        IFriendService friendService, 
        ClaimsPrincipal claim, 
        Guid friendId,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        if(!await friendService.RemoveFriendForUser(userId, friendId))
            return TypedResults.NotFound();
        
        await bus.Publish(new FriendRemovedMessage { UserId = userId,FriendId = friendId });
        return TypedResults.Ok();
    }

    private async Task<Results<Ok<List<UserResponse>>, NotFound>> GetFriendsForUser(
        IFriendService friendService, 
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var friends = await friendService.GetFriendsForUser(userId);
        if (friends is null) return TypedResults.NotFound();

        return TypedResults.Ok(friends.Select(f => f.ToUserResponse()).ToList());
    }
}