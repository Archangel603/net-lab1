using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Shared.Message;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;
using Shared.Socket;

namespace Client.Socket;

public class SocketService
{
    private SocketConnection _connection;
    private Guid? _sessionKey = null;
    private readonly ConcurrentDictionary<MessageType, List<Func<Message, Task>>> _eventHandlers = new();

    public UserInfo User { get; private set; }
    
    public void Authenticate(Guid sessionKey, UserInfo userInfo)
    {
        this._sessionKey = sessionKey;
        this.User = userInfo;
    }
    
    public async Task Connect(string address, int port)
    {
        var addresses = await Dns.GetHostAddressesAsync(address);

        if (addresses.Length == 0)
        {
            throw new Exception("Host not found");
        }

        var socket = new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(5000);

        try
        {
            await socket.ConnectAsync(addresses, port, cts.Token);
        }
        catch (OperationCanceledException e)
        {
            throw new Exception("Connection timeout");
        }

        this._connection = new SocketConnection(socket);
        this.listenMessages();
    }

    public async Task RequestChats()
    {
        await this.Send(MessageType.GetChatsRequest, new GetChatsRequest());
    }

    public async Task SendAuthMessage(AuthRequest message)
    {
        await this.Send(MessageType.AuthRequest, message);
    }
    
    public async Task SendRegisterMessage(RegisterRequest message)
    {
        await this.Send(MessageType.RegisterRequest, message);
    }
    
    public async Task Send<T>(MessageType messageType, T body)
    {
        if (this._sessionKey.HasValue && body is AuthenticatedRequest r)
        {
            r.SessionKey = this._sessionKey.Value;
        }
        
        await this._connection.WriteMessage(messageType, body);
    }
    
    public void Subscribe(MessageType messageType, Func<Message, Task> handler)
    {
        if (!this._eventHandlers.ContainsKey(messageType))
        {
            this._eventHandlers[messageType] = new();
        }
        
        this._eventHandlers[messageType].Add(handler);
    }
    
    public void Unsubscribe(MessageType messageType, Func<Message, Task> handler)
    {
        if (this._eventHandlers.ContainsKey(messageType))
        {
            this._eventHandlers[messageType].Remove(handler);
        }
    }

    private async Task listenMessages()
    {
        while (true)
        {
            try
            {
                var message = await this._connection.ReadMessage();
            
                if (!this._eventHandlers.ContainsKey(message.Header.Type))
                    continue;
                
                foreach (var handler in this._eventHandlers[message.Header.Type])
                {
                    await handler(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}