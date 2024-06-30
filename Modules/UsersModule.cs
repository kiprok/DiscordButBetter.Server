using Carter;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
namespace DiscordButBetter.Server.Modules;

public class UsersModule() : CarterModule("/api/users")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (DbbContext db) =>
        {
            var users = db.Users.ToList();
            return Results.Ok(users);
        });
        
        app.MapPut("/", (DbbContext db) =>
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
            db.SaveChanges();
            
            return Results.Ok(user);
        });
    }
}