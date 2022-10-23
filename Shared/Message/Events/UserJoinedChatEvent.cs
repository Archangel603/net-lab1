using Shared.Model;

namespace Shared.Message.Events;

public class UserJoinedChatEvent : IEvent
{
    public Guid ChatId { get; set; }
    
    public UserInfo User { get; set; }

    public UserJoinedChatEvent()
    {
        
    }
    
    public UserJoinedChatEvent(Guid chatId, UserInfo user)
    {
        this.ChatId = chatId;
        this.User = user;
    }
}