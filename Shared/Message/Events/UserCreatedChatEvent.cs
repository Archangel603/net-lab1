using Shared.Model;

namespace Shared.Message.Events;

public class UserCreatedChatEvent : IEvent
{
    public Guid ChatId { get; set; }

    public List<UserInfo> Users { get; set; } = new();
}