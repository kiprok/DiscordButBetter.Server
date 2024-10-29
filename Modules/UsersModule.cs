using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Services;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiscordButBetter.Server.Modules;

public class UsersModule : CarterModule
{
    public UsersModule() : base("/api/users")
    {
        RequireAuthorization();
        IncludeInOpenApi();
        WithTags("Users");
    }


    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetUser);

        app.MapGet("/{id:guid}", GetUserById);

        app.MapPatch("/", UpdateUser);

        app.MapGet("/search", SearchUsers);
    }

    private async Task<Ok<List<UserResponse>>> SearchUsers(
        IUserService userService, 
        [FromQuery] string query, 
        ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var users = await userService.SearchUsersByUserName(query, userId);
        
        return TypedResults.Ok(users.Select(u => u.ToUserResponse()).ToList());
    }

    private async Task<Results<Ok<UserUpdateResponse>, NotFound>> UpdateUser(
        ClaimsPrincipal claim,
        [FromBody] UpdateUserInfoRequest request,
        IUserService userService,
        IBus bus)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        
        if (!(await userService.UpdateUser(userId, request)))
            return TypedResults.NotFound();

        var response = new UserUpdateResponse
        {
            UserId = userId
        };
        if (request.ProfilePicture != null) response.ProfilePicture = request.ProfilePicture;
        if (request.Status != null) response.Status = request.Status.Value;
        if (request.StatusMessage != null) response.StatusMessage = request.StatusMessage;
        if (request.Biography != null) response.Biography = request.Biography;

        
        await bus.Publish(response.ToUserInfoChangedMessage());
        
        return TypedResults.Ok(response);
    }

    private async Task<Results<Ok<UserResponse>, NotFound>> GetUserById(IUserService userService, Guid id)
    {
        var user = await userService.GetUserById(id);
        if (user == null) 
            return TypedResults.NotFound();

        return TypedResults.Ok(user.ToUserResponse());
    }

    private async Task<Results<Ok<UserResponse>, NotFound>> GetUser(IUserService userService, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var user = await userService.GetUserById(userId);
        if (user == null) return TypedResults.NotFound();

        return TypedResults.Ok(user.ToUserResponse());
    }
}