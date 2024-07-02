using Carter;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
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
        
        app.MapPost("/", async (DbbContext db) =>
        {
            var user = new UserModel
            {
                Username = $"User{Random.Shared.Next(1000, 9999)}",
                Password = "password",
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
        
        app.MapGet("/{id:guid}", (DbbContext db, Guid id) =>
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.ToUserResponse());
        });
        
        app.MapGet("/friends/{id:guid}", (DbbContext db, Guid id) =>
        {
            var user = db.Users.Include(u => u.Friends).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(user.Friends.Select(f => f.ToUserResponse()));
        });
        
        
    }
}