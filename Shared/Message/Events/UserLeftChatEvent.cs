using Shared.Model;

namespace Shared.Message.Events;

public class UserLeftChatEvent : IEvent
{
    public Guid ChatId { get; set; }
    
    public UserInfo User { get; set; }
}