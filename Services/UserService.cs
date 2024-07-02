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

public class UserService : IUserService
{
    private readonly DbbContext _db;
    private readonly IMemoryCache _cache;
    
    public UserService(DbbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }
    
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
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
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
            
        _cache.Set(session.token, session, cacheEntryOptions);
        
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        
        
        return session;
    }
    
    public SessionModel? Authenticate(string token)
    {
        if (!_cache.TryGetValue(token, out SessionModel? session))
        {
            session = _db.Sessions.FirstOrDefault(s => s.token == token);
            if (session == null)
                return null;
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            
            _cache.Set(token, session, cacheEntryOptions);
            
        }
        
        return session;
    }

    public async Task<bool> Logout(string token)
    {
        if (!_cache.TryGetValue(token, out SessionModel? session))
        {
            session = _db.Sessions.FirstOrDefault(s => s.token == token);
            if (session == null)
                return false;
        }
        
        _db.Sessions.Remove(session);
        _cache.Remove(token);
        await _db.SaveChangesAsync();
        
        return true;
    }
}