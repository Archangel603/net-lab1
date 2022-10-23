using Server.Socket;
using Shared.Message.Requests;

namespace Shared.Message;

public interface IRequestHandler
{
    
}

public interface IRequestHandler<T> : IRequestHandler where T : IRequest
{
    Task Handle(T request, SocketClient client);
}