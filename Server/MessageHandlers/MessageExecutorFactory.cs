using System.Reflection;
using Autofac;
using Server.Socket;

namespace Shared.Message;

public class MessageExecutorFactory
{
    private static readonly Dictionary<Type, Type> _messageHandlers = new();
    
    private readonly IComponentContext _container;

    public MessageExecutorFactory(IComponentContext container)
    {
        this._container = container;
    }

    public Func<Message, SocketClient, Task> CreateMessageExecutor(Type messageType)
    {
        return async (m, client) =>
        {
            var handlerType = _messageHandlers[messageType];
            var handler = this._container.Resolve(handlerType);
            var method = handlerType.GetMethod("Handle");

            await (method.Invoke(handler, new[] { m.Body.Read(), client }) as Task);
        };
    }

    public static List<Type> FindMessageHandlers()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => 
                type.IsClass 
                && !type.IsAbstract 
                && type.IsAssignableTo<IRequestHandler>()
        ).ToList();
    }
    
    public static void RegisterMessageHandler(Type messageType, Type registeredHandler)
    {
        _messageHandlers[messageType] = registeredHandler;
    }
}