using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Services;

public interface IFriendService
{
    public Task<List<UserModel>?> GetFriendsForUser(Guid userId);
    
    public Task<bool> RemoveFriendForUser(Guid userId, Guid friendId);
    
    public Task<List<FriendRequestModel>?> GetFriendRequestsForUser(Guid userId);
    
    public Task<FriendRequestModel?> SendFriendRequest(Guid userId, Guid targetId);
    
    public Task<FriendRequestModel?> AcceptFriendRequest(Guid userId, Guid requestId);
    
    public Task<FriendRequestModel?> DeclineFriendRequest(Guid userId, Guid requestId);
    
    public Task<FriendRequestModel?> CancelFriendRequest(Guid userId, Guid requestId);
}

public class FriendService(DbbContext db) : IFriendService
{
    public Task<List<UserModel>?> GetFriendsForUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFriendForUser(Guid userId, Guid friendId)
    {
        throw new NotImplementedException();
    }

    public Task<List<FriendRequestModel>?> GetFriendRequestsForUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<FriendRequestModel?> SendFriendRequest(Guid userId, Guid targetId)
    {
        var request = new FriendRequestModel {SenderId = userId, ReceiverId = targetId};
        db.FriendRequests.Add(request);
        await db.SaveChangesAsync();
        return request;
    }

    public async Task<FriendRequestModel?> AcceptFriendRequest(Guid userId, Guid requestId)
    {
        var request = await db.FriendRequests.FindAsync(requestId);
        if (request is null) return null;
        
        if(request.ReceiverId != userId) 
            return null;
        
        var user = await db.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
        var targetUser = await db.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == request.SenderId);
        
        if(user is null || targetUser is null) return null;
        
        user.Friends.Add(targetUser);
        targetUser.Friends.Add(user);
        db.FriendRequests.Remove(request);
        await db.SaveChangesAsync();
        
        return request;
    }

    public async Task<FriendRequestModel?> DeclineFriendRequest(Guid userId, Guid requestId)
    {
        var request = await db.FriendRequests.FindAsync(requestId);
        if (request is null) return null;

        if (request.ReceiverId != userId)
            return null;
        
        db.FriendRequests.Remove(request);
        await db.SaveChangesAsync();
        
        return request;
    }

    public async Task<FriendRequestModel?> CancelFriendRequest(Guid userId, Guid requestId)
    {
        var request = await db.FriendRequests.FindAsync(requestId);
        if (request is null) return null;
        
        if(request.SenderId != userId) 
            return null;
        
        db.FriendRequests.Remove(request);
        await db.SaveChangesAsync();
        
        return request;
    }
}