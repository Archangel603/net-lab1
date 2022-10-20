using Shared.Model;

namespace Shared.Message.Events;

public class UserOnlineEvent : IEvent
{
    public MessageType MessageType => MessageType.UserOnline;
    
    public UserInfo User { get; set; }
}