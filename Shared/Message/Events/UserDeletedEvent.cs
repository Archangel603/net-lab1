namespace Shared.Message.Events;

public class UserDeletedEvent : IEvent
{
    public Guid UserId { get; set; }

    public UserDeletedEvent()
    {
        
    }
    
    public UserDeletedEvent(Guid userId)
    {
        this.UserId = userId;
    }
}