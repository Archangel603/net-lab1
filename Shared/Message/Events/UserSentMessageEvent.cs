using Shared.Model;

namespace Shared.Message.Events;

public class UserSentMessageEvent : IEvent
{
    public Guid ChatId { get; set; }
    
    public MessageInfo Message { get; set; }
}