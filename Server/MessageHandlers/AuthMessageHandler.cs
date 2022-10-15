using Server.Socket;
using Shared.Message;

namespace Server.MessageHandlers;

public class AuthMessageHandler : IMessageHandler
{
    public AuthMessageHandler()
    {

    }

    public async Task HandleMessage(Message message, SocketClient client)
    {
        var authMessage = message.Body.Read<AuthMessage>();

        if (authMessage.Username == "")
        {
            await client.Connection.WriteMessage(MessageType.Error, "");
            return;
        }
    }
}