using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using DiscordButBetter.Server.Authentication;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Modules;

public class AuthenticationModule : CarterModule
{
    public AuthenticationModule() : base("/api/auth")
    {
        
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/registration", async (
            [FromBody] RegistrationRequest request,
            DbbContext db,
            IUserService userService,
            HttpClient client) =>
        {
            if (await db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return Results.BadRequest("Username already exists.");
            }

            var randomProfilePicture = "";
            var randomName = "";
            var response = await client.GetAsync("https://randomuser.me/api/");
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var rUser = JsonNode.Parse(content);
                randomProfilePicture = rUser["results"][0]["picture"]["large"].ToString();
                randomName = rUser["results"][0]["name"]["first"].ToString();
                randomName += " " + rUser["results"][0]["name"]["last"].ToString();
            }
            else
            {
                return Results.BadRequest("Failed to generate random user.");
            }
            
            
            var user = new UserModel
            {
                Username = request.Username,
                Password = userService.GeneratePasswordHash(request.Password),
                CreatedAt = DateTime.Now,
                Status = 0,
                ProfilePicture = randomProfilePicture,
                StatusMessage = $"My name is {randomName}!",
                Biography = $"This is my very long biography. I am {randomName}.\n I am a new user.\n I am a very cool person."
            };
            
            db.Users.Add(user);
            await db.SaveChangesAsync();
            
            return Results.Ok(user.ToUserResponse());
        });

        app.MapPost("/login", async (
            [FromBody] LoginRequest request, 
            IUserService userService) =>
        {
            var session = await userService.Authenticate(request.Username, request.Password);
            if (session == null)
            {
                return Results.Unauthorized();
            }
            
            return Results.Ok( session.ToSessionResponse());
        });
        
        app.MapPost("/logout", async (
            [FromHeader(Name = AuthSchemeOptions.AuthorizationHeaderName)] string token, 
            IUserService userService) =>
        {
            var session = userService.Authenticate(token);
            if (session == null)
            {
                return Results.Unauthorized();
            }
            
            if(await userService.Logout(token))
            {
                return Results.Ok();
            }

            return Results.Unauthorized();
        });
    }
}