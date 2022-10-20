namespace Shared.Message.Events;

public class UserOfflineEvent : IEvent
{
    public MessageType MessageType => MessageType.UserOffline;
    
    public Guid UserId { get; set; }
}