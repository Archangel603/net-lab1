using System.Reflection;
using Autofac;
using Server.Socket;
using Shared.Message;

namespace Server.RequestHandlers;

public class RequestExecutorFactory
{
    private static readonly Dictionary<Type, Type> _requestHandlers = new();
    
    private readonly IComponentContext _container;

    public RequestExecutorFactory(IComponentContext container)
    {
        this._container = container;
    }

    public Func<Message, SocketClient, Task> CreateRequestExecutor(Type messageType)
    {
        return async (m, client) =>
        {
            var handlerType = _requestHandlers[messageType];
            var handler = this._container.Resolve(handlerType);
            var method = handlerType.GetMethod("Handle");

            await (method.Invoke(handler, new[] { m.Body.Read(), client }) as Task);
        };
    }

    public static List<Type> FindRequestHandlers()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => 
                type.IsClass 
                && !type.IsAbstract 
                && type.IsAssignableTo<IRequestHandler>()
        ).ToList();
    }
    
    public static void RegisterRequestHandler(Type messageType, Type registeredHandler)
    {
        _requestHandlers[messageType] = registeredHandler;
    }
}