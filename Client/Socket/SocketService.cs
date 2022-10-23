using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Shared.Message.Requests;
using Shared.Model;
using Shared.Socket;

namespace Client.Socket;

public class SocketService
{
    private SocketConnection _connection;
    private Guid? _sessionKey = null;
    private readonly ConcurrentDictionary<Type, List<EventListener>> _eventListeners = new();

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

    public async Task Send<T>(T body) where T : IRequest
    {
        if (this._sessionKey.HasValue && body is AuthenticatedRequest r)
        {
            r.SessionKey = this._sessionKey.Value;
        }
        
        await this._connection.WriteMessage(body);
    }
    
    public EventListener Subscribe<T>(Func<T, Task> handler)
    {
        var messageType = typeof(T);
        
        if (!this._eventListeners.ContainsKey(messageType))
        {
            this._eventListeners[messageType] = new();
        }

        var listener = new EventListener(handler);
        
        this._eventListeners[messageType].Add(listener);

        return listener;
    }
    
    public void Unsubscribe<T>(Func<T, Task> handler)
    {
        var messageType = typeof(T);
        
        if (this._eventListeners.ContainsKey(messageType))
        {
            var existing = this._eventListeners[messageType]
                .FirstOrDefault(e => e.Handler.Equals(handler));

            if (existing is not null)
            {
                this._eventListeners[messageType].Remove(existing);
            }
        }
    }

    public void Unsubscribe(Type messageType, EventListener listener)
    {
        if (this._eventListeners.ContainsKey(messageType))
        {
            this._eventListeners[messageType].Remove(listener);
        }
    }

    private async Task listenMessages()
    {
        while (true)
        {
            try
            {
                var message = await this._connection.ReadMessage();
                var messageType = message.Body.GetBodyType();
            
                if (!this._eventListeners.ContainsKey(messageType))
                    continue;
                
                foreach (var listener in this._eventListeners[messageType])
                {
                    await listener.Execute(message.Body.Read());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}