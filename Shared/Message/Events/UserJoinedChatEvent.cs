using Shared.Model;

namespace Shared.Message.Events;

public class UserJoinedChatEvent : IEvent
{
    public MessageType MessageType => MessageType.UserJoinedChat;
    
    public Guid ChatId { get; set; }
    
    public UserInfo User { get; set; }
}