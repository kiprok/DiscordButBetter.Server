using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Services;

public interface IMessageService
{
    public Task<ChatMessageModel?> GetMessageById(Guid messageId);
    public Task<List<ChatMessageModel>> GetMessagesFromConversation(Guid conversationId);
    public Task<List<ChatMessageModel>> GetOlderMessages(Guid conversationId, int total, DateTime messageTime);
    public Task<List<ChatMessageModel>> GetNewerMessages(Guid conversationId, int total, DateTime messageTime);
    public Task<ChatMessageModel> CreateNewMessage(ChatMessageModel message);
    public Task<bool> DeleteMessageById(Guid messageId);
    public Task<bool> UpdateMessageById(ChatMessageModel message);
    public Task<List<ChatMessageModel>?> SearchForMessages(Guid conversationId, string query);
}

public class MessageService(DbbContext db) : IMessageService
{
    public async Task<ChatMessageModel?> GetMessageById(Guid messageId)
    {
        return await db.Messages.FindAsync(messageId);
    }

    public async Task<List<ChatMessageModel>> GetMessagesFromConversation(Guid conversationId)
    {
        return await db.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task<List<ChatMessageModel>> GetOlderMessages(Guid conversationId, int total, DateTime messageTime)
    {
        return await db.Messages
            .Where(m => m.ConversationId == conversationId && m.SentAt < messageTime)
            .OrderByDescending(m => m.SentAt)
            .Take(total)
            .ToListAsync();
    }

    public async Task<List<ChatMessageModel>> GetNewerMessages(Guid conversationId, int total, DateTime messageTime)
    {
        return await db.Messages
            .Where(m => m.ConversationId == conversationId && m.SentAt > messageTime)
            .OrderBy(m => m.SentAt)
            .Take(total)
            .ToListAsync();
    }

    public async Task<ChatMessageModel> CreateNewMessage(ChatMessageModel message)
    {
        db.Messages.Add(message);
        var conversation = await db.Conversations.FindAsync(message.ConversationId);
        conversation!.LastMessageTime = message.SentAt;
        await db.SaveChangesAsync();
        return message;
    }

    public async Task<bool> DeleteMessageById(Guid messageId)
    {
        var result = await db.Messages.Where(x => x.Id == messageId).ExecuteDeleteAsync();
        return result == 1;
    }

    public async Task<bool> UpdateMessageById(ChatMessageModel message)
    {
        db.Messages.Update(message);
        return await db.SaveChangesAsync() == 1;
    }

    public Task<List<ChatMessageModel>?> SearchForMessages(Guid conversationId, string query)
    {
        throw new NotImplementedException();
    }
}