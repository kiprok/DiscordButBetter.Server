using Carter;
using DiscordButBetter.Server.Authentication;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscordButBetter.Server.Modules;

public class AuthenticationModule : CarterModule
{
    public AuthenticationModule() : base("/api/auth")
    {
        
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/registration", async ([FromBody] RegistrationRequest request,DbbContext db, IUserService userService) =>
        {
            var user = new UserModel
            {
                Username = request.Username,
                Password = userService.GeneratePasswordHash(request.Password),
                CreatedAt = DateTime.Now,
                Status = 0,
                ProfilePicture = "https://i.imgur.com/Y86bvSa.jpeg",
                StatusMessage = "Hello, world!",
                Biography = "This is a user's biography."
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