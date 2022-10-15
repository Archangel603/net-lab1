using Server.Socket;
using Shared.Message;

namespace Server.MessageHandlers;

public interface IMessageHandler
{
    Task HandleMessage(Message message, SocketClient client);
}