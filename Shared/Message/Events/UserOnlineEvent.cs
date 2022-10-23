using Shared.Model;

namespace Shared.Message.Events;

public class UserOnlineEvent : IEvent
{
    public UserInfo User { get; set; }
}