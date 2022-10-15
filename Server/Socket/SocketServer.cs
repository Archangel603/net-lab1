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
            
            this._clients[client.Id] = client;
            this._listeners[client.Id] = cancellationTokenSource;
            this.listenClient(client, cancellationTokenSource.Token);
        }
    }

    private async Task listenClient(SocketClient client, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var message = await client.Connection.ReadMessage();
                var handler = this._messageHandlerFactory.Create(message.Header.Type);
                await handler.HandleMessage(message, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private async Task monitorConnections()
    {
        while (true)
        {
            foreach (var (id, client) in this._clients)
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
        this._listeners[client.Id].Cancel();
    }
}