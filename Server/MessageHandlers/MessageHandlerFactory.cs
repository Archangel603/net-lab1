using Autofac;
using Server.MessageHandlers;

namespace Shared.Message;

public class MessageHandlerFactory
{
    private readonly IComponentContext _context;
    private readonly Dictionary<MessageType, Type> _messageHandlers = new();

    public MessageHandlerFactory(IComponentContext context)
    {
        this._context = context;
    }

    public IMessageHandler Create(MessageType messageType)
    {
        return this._context.Resolve(this._messageHandlers[messageType]) as IMessageHandler;
    }

    public MessageHandlerFactory WithHandler<T>(MessageType messageType)
    {
        this._messageHandlers[messageType] = typeof(T);
        return this;
    }
}