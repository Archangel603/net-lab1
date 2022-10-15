using Shared.Socket;

namespace Server.Socket;

public class SocketClient
{
    public delegate SocketClient Factory(Guid id, SocketConnection connection);
    
    public SocketConnection Connection { get; }
    
    public Guid Id { get; }
    
    public SocketClient(Guid id, SocketConnection connection)
    {
        this.Id = id;
        this.Connection = connection;
    }
}