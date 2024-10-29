using System.Security.Cryptography;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiscordButBetter.Server.Services;

public interface IUserService
{
    public string GeneratePasswordHash(string password);
    public bool VerifyPassword(string password, string hash);
    public string GenerateToken();
    public Task<SessionModel?> Authenticate(string username, string password, string ip, string userAgent);
    public SessionModel? Authenticate(string token);
    
    public Task<UserModel?> RegisterUser(string username, string password);
    
    public Task<UserModel?> GetUserById(Guid id);

    public Task<bool> Logout(string token);
}

public class UserService(DbbContext db, IMemoryCache cache) : IUserService
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string GeneratePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateToken()
    {
        return RandomNumberGenerator.GetString(Chars, 64);
    }

    public async Task<SessionModel?> Authenticate(string username, string password, string ip, string userAgent)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || !VerifyPassword(password, user.Password)) return null;

        var session = new SessionModel
        {
            userId = user.Id,
            token = GenerateToken(),
            IpAddress = ip,
            UserAgent = userAgent
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));

        cache.Set(session.token, session, cacheEntryOptions);
        
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        return session;
    }

    public SessionModel? Authenticate(string token)
    {
        if (cache.TryGetValue(token, out SessionModel? session)) return session;
        
        session = db.Sessions.FirstOrDefault(s => s.token == token);
        
        if (session == null)
            return null;

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));

        cache.Set(token, session, cacheEntryOptions);

        return session;
    }

    public async Task<bool> Logout(string token)
    {
        if (!cache.TryGetValue(token, out SessionModel? session))
        {
            session = db.Sessions.FirstOrDefault(s => s.token == token);
            if (session == null)
                return false;
        }

        await db.Sessions.Where(x => x.Id == session!.Id).ExecuteDeleteAsync();
        cache.Remove(token);

        return true;
    }
    
    public async Task<UserModel?> RegisterUser(string username, string password)
    {
        if(db.Users.FirstOrDefault(x => x.Username == username) != null)
            return null;
        
        var user = new UserModel
        {
            Username = username,
            Password = GeneratePasswordHash(password),
            CreatedAt = DateTime.UtcNow,
            Status = 0,
            ProfilePicture = "",
            StatusMessage = $"My name is {username}!",
            Biography =
                $"This is my very long biography. I am {username}.\n I am a new user.\n I am a very cool person."
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }
    
    public async Task<UserModel?> GetUserById(Guid id)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }
}