using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Shared.Message;
using Shared.Socket;

namespace Client.Socket;

public class SocketService
{
    private SocketConnection _connection;
    private readonly ConcurrentDictionary<MessageType, List<Func<Message, Task>>> _eventHandlers = new();

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

    public async Task SendAuthMessage(AuthRequest message)
    {
        await this.Send(MessageType.AuthRequest, message);
    }
    
    public async Task Send<T>(MessageType messageType, T body)
    {
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
            var message = await this._connection.ReadMessage();
            
            Task.Run(async () =>
            {
                foreach (var handler in this._eventHandlers[message.Header.Type])
                {
                    await handler(message);
                }
            });
        }
    }
}