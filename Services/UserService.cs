using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DiscordButBetter.Server.Services;

public interface IUserService
{
    public string GeneratePasswordHash(string password);
    public bool VerifyPassword(string password, string hash);
    public string GenerateToken();
    public Task<SessionModel?> Authenticate(string username, string password , string ip, string userAgent);
    public SessionModel? Authenticate(string token);
    
    public Task<bool> Logout(string token);
}

public class UserService(DbbContext db, IMemoryCache cache) : IUserService
{
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
        return Guid.NewGuid().ToString();
    }
    
    public async Task<SessionModel?> Authenticate(string username, string password, string ip, string userAgent)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || !VerifyPassword(password, user.Password))
        {
            return null;
        }
        
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
        if (!cache.TryGetValue(token, out SessionModel? session))
        {
            session = db.Sessions.FirstOrDefault(s => s.token == token);
            if (session == null)
                return null;
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            
            cache.Set(token, session, cacheEntryOptions);
            
        }
        
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
        
        db.Sessions.Remove(session);
        cache.Remove(token);
        await db.SaveChangesAsync();
        
        return true;
    }
}