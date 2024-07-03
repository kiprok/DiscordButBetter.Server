using System.Security.Claims;
using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Services;
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
        app.MapGet("/",  
            (DbbContext db, ClaimsPrincipal claim) =>
        { 
            var userId = Guid.Parse(claim.Claims.First().Value);
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(user.ToUserResponse());
        });
        
        app.MapGet("/{id:guid}", 
            (DbbContext db, Guid id) =>
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.ToUserResponse());
        });

        app.MapPatch("/", 
            async (
            DbbContext db,
            ClaimsPrincipal claim,
            UpdateUserInfoRequest request,
            IUserService userService) =>
        {
            var userId = Guid.Parse(claim.Claims.First().Value);
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return Results.NotFound();
            }
            if (request.Password != null) user.Password = userService.GeneratePasswordHash(request.Password);
            if (request.ProfilePicture != null) user.ProfilePicture = request.ProfilePicture;
            if (request.Status != null) user.Status = request.Status.Value;
            if (request.StatusMessage != null) user.StatusMessage = request.StatusMessage;
            if (request.Biography != null) user.Biography = request.Biography;
            await db.SaveChangesAsync();
            return Results.Ok(user.ToUserResponse());
        });

    }
}