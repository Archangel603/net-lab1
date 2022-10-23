namespace Shared.Message.Events;

public class UserOfflineEvent : IEvent
{
    public Guid UserId { get; set; }
}