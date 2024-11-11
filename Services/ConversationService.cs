using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Services;

public interface IConversationService
{
    public Task<List<ConversationModel>?> GetConversationsForUser(Guid userId);
    public Task<List<ConversationModel>?> GetVisibleConversationsForUser(Guid userId);
    public Task<ConversationModel?> GetConversationById(Guid conversationId);
    public Task<bool> DeleteVisibleConversationById(Guid userId, Guid conversationId);
    public Task<bool> AddUserToVisibleConversation(Guid userId, Guid conversationId);
    public Task<ConversationModel?> DeleteConversationById(Guid userId, Guid conversationId);
    public Task<ConversationModel?> CreateNewConversation(Guid userId, CreateConversationRequest request);
    public Task<ConversationModel?> GetPrivateConversation(Guid userId, Guid otherUserId);

    public Task<ConversationModel?> UpdateConversationById(Guid userId, Guid conversationId,
        UpdateConversationRequest request);
}

public class ConversationService(DbbContext db) : IConversationService
{
    public async Task<List<ConversationModel>?> GetConversationsForUser(Guid userId)
    {
        return await db.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Include(u => u.Conversations)
            .ThenInclude(c => c.Participants)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Conversations)
            .ToListAsync();
    }

    public async Task<List<ConversationModel>?> GetVisibleConversationsForUser(Guid userId)
    {
        return await db.Users
            .AsNoTracking()
            .Include(u => u.VisibleConversations)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.VisibleConversations)
            .ToListAsync();
    }

    public async Task<ConversationModel?> GetConversationById(Guid conversationId)
    {
        return await db.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<bool> DeleteVisibleConversationById(Guid userId, Guid conversationId)
    {
        var user = await db.Users
            .Include(u => u.VisibleConversations)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return false;

        var conversation = user.VisibleConversations.FirstOrDefault(c => c.Id == conversationId);
        if (conversation is null) return false;

        user.VisibleConversations.Remove(conversation);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddUserToVisibleConversation(Guid userId, Guid conversationId)
    {
        var user = await db.Users
            .Include(u => u.VisibleConversations)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return false;

        var conversation = await db.Conversations
            .Include(c => c.Participants)
            .Where(c => c.Participants.FirstOrDefault(u => u.Id == userId) != null)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation is null) return false;

        user.VisibleConversations.Add(conversation);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ConversationModel?> DeleteConversationById(Guid userId, Guid conversationId)
    {
        var user = db.Users
            .Include(u => u.VisibleConversations)
            .FirstOrDefault(u => u.Id == userId);

        var conversation = db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefault(c => c.Id == conversationId);

        if (conversation == null || user == null) return null;

        if (conversation.Participants.FirstOrDefault(u => u.Id == userId) == null) return null;

        if (conversation.ConversationType == 0)
        {
            user.VisibleConversations.Remove(conversation);
            await db.SaveChangesAsync();
            return conversation;
        }

        conversation.Participants.Remove(user);
        if (conversation.Participants.Count == 1)
        {
            db.Conversations.Remove(conversation);
            await db.SaveChangesAsync();
            return conversation;
        }

        if (conversation.OwnerId == userId) conversation.OwnerId = conversation.Participants.First().Id;

        await db.SaveChangesAsync();
        return conversation;
    }

    public async Task<ConversationModel?> CreateNewConversation(Guid userId, CreateConversationRequest request)
    {
        var participants = await db.Users.Where(u => request.Participants.Contains(u.Id)).ToListAsync();
        participants.Add(db.Users.FirstOrDefault(u => u.Id == userId)!);

        var conversation = new ConversationModel
        {
            OwnerId = userId,
            ConversationName = request.ConversationName,
            ConversationType = request.ConversationType,
            ConversationPicture = "",
            Participants = participants,
            ParticipantsVisible = participants
        };

        db.Conversations.Add(conversation);
        await db.SaveChangesAsync();
        return conversation;
    }

    public async Task<ConversationModel?> GetPrivateConversation(Guid userId, Guid otherUserId)
    {
        return await db.Conversations
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Participants)
            .Include(c => c.ParticipantsVisible)
            .FirstOrDefaultAsync(c =>
                c.ConversationType == 0 &&
                c.Participants.FirstOrDefault(u => u.Id == userId) != null &&
                c.Participants.FirstOrDefault(u => u.Id == otherUserId) != null);
    }

    public async Task<ConversationModel?> UpdateConversationById(Guid userId, Guid conversationId,
        UpdateConversationRequest request)
    {
        var conversation = db.Conversations
            .Include(c => c.Participants)
            .Include(c => c.ParticipantsVisible)
            .FirstOrDefault(c => c.Id == conversationId && c.Participants.FirstOrDefault(u => u.Id == userId) != null);
        if (conversation == null) return null;

        if (request.ConversationName != null) conversation.ConversationName = request.ConversationName;
        if (request.ConversationPicture != null) conversation.ConversationPicture = request.ConversationPicture;
        if (request.ParticipantsToAdd != null)
        {
            conversation.Participants.AddRange(db.Users.Where(u => request.ParticipantsToAdd.Contains(u.Id)));
            conversation.ParticipantsVisible.AddRange(db.Users.Where(u => request.ParticipantsToAdd.Contains(u.Id)));
        }

        if (request.ParticipantsToRemove != null)
        {
            conversation.Participants.RemoveAll(u => request.ParticipantsToRemove.Contains(u.Id));
            conversation.ParticipantsVisible.RemoveAll(u => request.ParticipantsToRemove.Contains(u.Id));
        }

        await db.SaveChangesAsync();
        return conversation;
    }
}