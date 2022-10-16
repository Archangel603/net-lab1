using Autofac;
using Server.MessageHandlers;

namespace Shared.Message;

public class MessageHandlerFactory
{
    private readonly IComponentContext _container;
    private readonly Dictionary<MessageType, Type> _messageHandlers = new();

    public MessageHandlerFactory(IComponentContext container)
    {
        this._container = container;
    }

    public IMessageHandler Create(MessageType messageType)
    {
        return this._container.Resolve(this._messageHandlers[messageType]) as IMessageHandler;
    }

    public MessageHandlerFactory WithHandler<T>(MessageType messageType)
    {
        this._messageHandlers[messageType] = typeof(T);
        return this;
    }
}