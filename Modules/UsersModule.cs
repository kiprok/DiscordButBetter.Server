using Carter;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
namespace DiscordButBetter.Server.Modules;

public class UsersModule() : CarterModule("/api/users")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () =>
        {
            return Results.Ok(new[]
            {
                new { Id = 1, Name = "Alice" },
                new { Id = 2, Name = "Bob" },
                new { Id = 3, Name = "Charlie" }
            });
        });
        
        app.MapPut("/", (DbbContext db) =>
        {
            for (int i = 0; i < 100; i++)
            {
                db.Users.Add(new UserModel
                {
                    Username = $"User{i}",
                    Password = "password",
                    CreatedAt = DateTime.Now,
                    Status = 0,
                    ProfilePicture = "https://i.imgur.com/Y86bvSa.jpeg",
                    StatusMessage = "Hello, world!",
                    Biography = "This is a user's biography."
                });
            }
            db.SaveChanges();
            
            return Results.Ok();
        });
    }
}