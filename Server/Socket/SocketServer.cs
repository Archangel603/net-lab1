using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Shared.Message;
using Shared.Socket;

namespace Server.Socket;

public class SocketServer
{
    private ConcurrentDictionary<Guid, SocketClient> _clients = new();
    private ConcurrentDictionary<Guid, CancellationTokenSource> _listeners = new();
    private System.Net.Sockets.Socket _socket;
    private readonly MessageHandlerFactory _messageHandlerFactory;
    private readonly SocketClient.Factory _clientFactory;

    public SocketServer(SocketClient.Factory clientFactory, MessageHandlerFactory messageHandlerFactory)
    {
        this._clientFactory = clientFactory;
        this._messageHandlerFactory = messageHandlerFactory;
    }
    
    public async Task Start(int port)
    {
        this._socket = new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
        this._socket.Bind(new IPEndPoint(IPAddress.Any, port));
        this._socket.Listen();

        Console.WriteLine($"Server is listening on TCP *:{port}");
        
        this.monitorConnections();
        await this.handleConnections();
    }

    private async Task handleConnections()
    {
        while (true)
        {
            var handler = await this._socket.AcceptAsync();
            var client = this._clientFactory(Guid.NewGuid(), new SocketConnection(handler));
            var cancellationTokenSource = new CancellationTokenSource();
            
            Console.WriteLine($"New client {client.Id} connected");
            
            this._clients[client.Id] = client;
            this._listeners[client.Id] = cancellationTokenSource;
            this.listenClient(client, cancellationTokenSource.Token);
        }
    }

    private async Task listenClient(SocketClient client, CancellationToken cancellationToken)
    {
        var fails = 0;
        
        while (!cancellationToken.IsCancellationRequested && fails < 10)
        {
            try
            {
                var message = await client.Connection.ReadMessage();
                var handler = this._messageHandlerFactory.Create(message.Header.Type);
                await handler.HandleMessage(message, client);
                fails = 0;
            }
            catch (SocketException)
            {
                fails++;
                await Task.Delay(500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        if (fails >= 10)
        {
            Console.WriteLine($"Client {client.Id} timed out");
        }
    }

    private async Task monitorConnections()
    {
        while (true)
        {
            foreach (var (id, client) in this._clients.ToList())
            {
                if (!client.Connection.IsConnected())
                {
                    await this.handleDisconnected(client);
                }
            }
            
            await Task.Delay(1000);
        }
    }

    private async Task handleDisconnected(SocketClient client)
    {
        Console.WriteLine($"Client {client.Id} disconnected");
        this._listeners[client.Id].Cancel();
        this._clients.Remove(client.Id, out _);
    }
}