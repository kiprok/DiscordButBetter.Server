using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using DiscordButBetter.Server.Authentication;
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

public class AuthenticationModule : CarterModule
{
    public AuthenticationModule() : base("/api/auth")
    {
        IncludeInOpenApi();
        WithTags("Authentication");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/registration", RegisterNewUser);

        app.MapPost("/login", LoginUser);

        app.MapPost("/logout", LogoutUser);
    }

    private async Task<Results<Ok, UnauthorizedHttpResult>> LogoutUser(IUserService userService, HttpRequest request)
    {
        if (!request.Headers.ContainsKey(AuthSchemeOptions.AuthorizationHeaderName)) return TypedResults.Unauthorized();

        var token = request.Headers[AuthSchemeOptions.AuthorizationHeaderName];
        var session = userService.Authenticate(token);
        if (session == null) return TypedResults.Unauthorized();

        if (await userService.Logout(token)) return TypedResults.Ok();

        return TypedResults.Unauthorized();
    }

    private async Task<Results<Ok<SessionResponse>, UnauthorizedHttpResult>> LoginUser([FromBody] LoginRequest request,
        IUserService userService, HttpContext context)
    {
        
        var ip = context.Request.Headers["X-Forwarded-For"].Count > 0 ? 
            context.Request.Headers["X-Forwarded-For"].ToString() : 
            context.Connection.RemoteIpAddress!.ToString();
        
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        var session = await userService.Authenticate(request.Username, request.Password, ip ?? "", userAgent);
        if (session == null) return TypedResults.Unauthorized();

        return TypedResults.Ok(session.ToSessionResponse());
    }

    private async Task<Results<Ok<UserResponse>, BadRequest<string>>> RegisterNewUser(
        [FromBody] RegistrationRequest request, DbbContext db, IUserService userService, HttpClient client)
    {
        if (request.Username.Length < 3 || request.Username.Length > 20)
            return TypedResults.BadRequest("Username must be between 3 and 20 characters.");

        if (request.Password.Length < 8 || request.Password.Length > 50)
            return TypedResults.BadRequest("Password must be between 8 and 50 characters.");

        if (await db.Users.AnyAsync(u => u.Username == request.Username))
            return TypedResults.BadRequest("Username already exists.");

        var randomProfilePicture = "";
        var randomName = "";
        var response = await client.GetAsync("https://randomuser.me/api/");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var rUser = JsonNode.Parse(content);
            randomProfilePicture = rUser["results"][0]["picture"]["large"].ToString();
            randomName = rUser["results"][0]["name"]["first"].ToString();
            randomName += " " + rUser["results"][0]["name"]["last"].ToString();
        }
        else
        {
            return TypedResults.BadRequest("Failed to generate random user.");
        }


        var user = new UserModel
        {
            Username = request.Username,
            Password = userService.GeneratePasswordHash(request.Password),
            CreatedAt = DateTime.UtcNow,
            Status = 0,
            ProfilePicture = randomProfilePicture,
            StatusMessage = $"My name is {randomName}!",
            Biography =
                $"This is my very long biography. I am {randomName}.\n I am a new user.\n I am a very cool person."
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return TypedResults.Ok(user.ToUserResponse());
    }
}