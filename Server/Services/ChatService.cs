using Microsoft.EntityFrameworkCore;
using Server.Db;
using Server.Extensions;
using Server.Model;
using Shared.Message.Events;
using Shared.Model;

namespace Server.Services;

public class ChatService
{
    private readonly ServerDbContext _db;
    private readonly EventBus _eventBus;

    public ChatService(ServerDbContext db, EventBus eventBus)
    {
        this._db = db;
        this._eventBus = eventBus;
    }

    public async Task<List<Chat>> GetChats()
    {
        return await this._db.Chats.ToListAsync();
    }

    public async Task<Chat?> Find(Guid chatId)
    {
        return await this._db.Chats.FindAsync(chatId);
    }

    public async Task<List<Chat>> GetChats(Guid userId)
    {
        var user = await this._db.Users.FindAsync(userId);
        
        return await this._db.Chats.Where(c => c.Users.Contains(user)).ToListAsync();
    }

    public async Task<Chat?> GetPublicChat()
    {
        return await this._db.Chats.FirstOrDefaultAsync(c => c.Type == ChatType.Public);
    }

    public async Task<Message?> GetLastMessage(Guid chatId)
    {
        return await this._db.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Message>> GetMessages(Guid chatId)
    {
        return await this._db.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Message> AddMessage(Guid chatId, Guid senderId, string message)
    {
        var entry = this._db.Messages.Add(new Message
        {
            ChatId = chatId,
            SenderId = senderId,
            Text = message
        });

        await this._db.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Chat> EnsurePublicChatExists()
    {
        var chat = await this.GetPublicChat();

        if (chat is not null)
            return chat;

        return await this.CreateChat(ChatType.Public, new List<Guid>());
    }
    
    public async Task<Chat> CreatePrivateChat(Guid firstUserId, Guid secondUserId)
    {
        return await this.CreateChat(ChatType.Private, new List<Guid> { firstUserId, secondUserId });
    }
    
    public async Task<Chat> CreateChat(ChatType chatType, List<Guid> userIds)
    {
        var users = await this._db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        var entry = this._db.Chats.Add(new Chat
        {
            Type = chatType,
            Users = users
        });

        await this._db.SaveChangesAsync();
        return entry.Entity;
    }
    
    public async Task AddUserToChat(Guid chatId, Guid userId)
    {
        var chat = await this._db.Chats.FindAsync(chatId);
        var user = await this._db.Users.FindAsync(userId);
        
        chat.Users.Add(user);

        await this._db.SaveChangesAsync();

        await this._eventBus.PublishEvent(new UserJoinedChatEvent
        {
            ChatId = chat.Id,
            User = user.ToUserInfo()
        });
    }

    public async Task AddUserToPublicChat(User user)
    {
        var chat = await this.GetPublicChat();

        await this.AddUserToChat(chat.Id, user.Id);
    }
}