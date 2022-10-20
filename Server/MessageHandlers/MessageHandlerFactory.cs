using System.Reflection;
using Autofac;
using Server.MessageHandlers;

namespace Shared.Message;

public class MessageHandlerFactory
{
    private static readonly Dictionary<MessageType, Type> _messageHandlers = new();
    
    private readonly IComponentContext _container;

    public MessageHandlerFactory(IComponentContext container)
    {
        this._container = container;
    }

    public IMessageHandler Create(MessageType messageType)
    {
        return this._container.Resolve(_messageHandlers[messageType]) as IMessageHandler;
    }

    public static List<Type> FindMessageHandlers()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => 
                type.IsClass 
                && !type.IsAbstract 
                && type.IsAssignableTo<IMessageHandler>()
        ).ToList();
    }
    
    public static void RegisterMessageHandler(Type type)
    {
        var name = type.Name.Replace("Handler", "");
        _messageHandlers[Enum.Parse<MessageType>(name)] = type;
    }
}