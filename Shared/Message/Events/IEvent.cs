namespace Shared.Message.Events;

public interface IEvent
{
    public MessageType MessageType { get; }
}