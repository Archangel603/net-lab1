using Server.Socket;
using Shared.Message.Requests;

namespace Server.RequestHandlers;

public interface IRequestHandler
{
    
}

public interface IRequestHandler<T> : IRequestHandler where T : IRequest
{
    Task Handle(T request, SocketClient client);
}