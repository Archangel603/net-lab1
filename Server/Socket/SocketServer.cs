using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Server.RequestHandlers;
using Server.Services;
using Shared.Message;
using Shared.Message.Events;
using Shared.Socket;

namespace Server.Socket;

public class SocketServer
{
    private ConcurrentDictionary<Guid, SocketClient> _clients = new();
    private ConcurrentDictionary<Guid, CancellationTokenSource> _listeners = new();
    private System.Net.Sockets.Socket _socket;
    
    private readonly RequestExecutorFactory _requestExecutorFactory;
    private readonly SocketClient.Factory _clientFactory;
    private readonly EventBus _eventBus;
    private readonly ChatService _chatService;

    public SocketServer(
        SocketClient.Factory clientFactory,
        RequestExecutorFactory requestExecutorFactory,
        ChatService chatService,
        EventBus eventBus
    )
    {
        this._clientFactory = clientFactory;
        this._requestExecutorFactory = requestExecutorFactory;
        this._chatService = chatService;
        this._eventBus = eventBus;
    }

    public async Task Start(int port)
    {
        this._socket = new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
        this._socket.Bind(new IPEndPoint(IPAddress.Any, port));
        this._socket.Listen();

        Console.WriteLine($"Server is listening on TCP *:{port}");

        await this._chatService.EnsurePublicChatExists();

        this.monitorConnections();
        this.handleEvents();
        await this.handleConnections();
    }

    private async Task handleEvents()
    {
        await foreach (var e in this._eventBus.ListenForEvents(CancellationToken.None))
        {
            foreach (var (_, client) in this._clients)
            {
                try
                {
                    await client.Connection.WriteMessage(e);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }

    private async Task handleConnections()
    {
        while (true)
        {
            try
            {
                var handler = await this._socket.AcceptAsync();
                var client = this._clientFactory(Guid.NewGuid(), new SocketConnection(handler));
                var cancellationTokenSource = new CancellationTokenSource();

                Console.WriteLine($"New client {client.Id} connected");

                this._clients[client.Id] = client;
                this._listeners[client.Id] = cancellationTokenSource;
                this.listenClient(client, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
                
                Console.WriteLine($"New message of type {message.Body.GetBodyTypeName()} from client {client.Id}");
                
                var executor = this._requestExecutorFactory.CreateRequestExecutor(message.Body.GetBodyType());
                await executor(message, client);
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
                try
                {
                    if (!client.Connection.IsConnected())
                    {
                        await this.handleDisconnected(client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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

        await this._eventBus.PublishEvent(new UserOfflineEvent
        {
            UserId = client.Id
        });
    }
}