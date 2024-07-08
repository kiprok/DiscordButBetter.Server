﻿using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Services;
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

    private Ok<List<UserResponse>> SearchUsers(DbbContext db, [FromQuery] string query, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);

        var loweredQuery = query.ToLower();
        var users = db.Users.Where(u => u.Username.ToLower().Contains(loweredQuery) && u.Id != userId)
            .Take(10)
            .ToList();
        return TypedResults.Ok(users.Select(u => u.ToUserResponse()).ToList());
    }

    private async Task<Results<Ok<UserResponse>, NotFound>> UpdateUser(DbbContext db, ClaimsPrincipal claim,
        UpdateUserInfoRequest request,
        IUserService userService)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return TypedResults.NotFound();
        if (request.Password != null) user.Password = userService.GeneratePasswordHash(request.Password);
        if (request.ProfilePicture != null) user.ProfilePicture = request.ProfilePicture;
        if (request.Status != null) user.Status = request.Status.Value;
        if (request.StatusMessage != null) user.StatusMessage = request.StatusMessage;
        if (request.Biography != null) user.Biography = request.Biography;
        await db.SaveChangesAsync();
        return TypedResults.Ok(user.ToUserResponse());
    }

    private Results<Ok<UserResponse>, NotFound> GetUserById(DbbContext db, Guid id)
    {
        var user = db.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return TypedResults.NotFound();

        return TypedResults.Ok(user.ToUserResponse());
    }

    private Results<Ok<UserResponse>, NotFound> GetUser(DbbContext db, ClaimsPrincipal claim)
    {
        var userId = Guid.Parse(claim.Claims.First().Value);
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return TypedResults.NotFound();

        return TypedResults.Ok(user.ToUserResponse());
    }
}