using System.Net.Sockets;
using System.Text.Json;
using Shared.Message;

namespace Shared.Socket;

public class SocketConnection
{
    private readonly Mutex _readMutex = new();
    private readonly Mutex _writeMutex = new();
    private readonly System.Net.Sockets.Socket _connection;

    public SocketConnection(System.Net.Sockets.Socket connection)
    {
        this._connection = connection;
    }

    public async Task<Message.Message> ReadMessage()
    {
        this._readMutex.WaitOne();
        
        var bytes = new byte[9];
        var received = await this._connection.ReceiveAsync(bytes);

        if (bytes.Length != received)
        {
            throw new Exception("Unable to read header");
        }

        var header = MessageHeader.FromBytes(bytes);
        var buffer = new Memory<byte>(new byte[header.Length]);

        await this._connection.ReceiveAsync(buffer);
        
        this._readMutex.ReleaseMutex();

        return new Message.Message(header, new MessageBodyReader(buffer));
    }
    
    public async Task WriteMessage<T>(MessageType type, T message)
    {
        this._writeMutex.WaitOne();
        
        var serialized = JsonSerializer.SerializeToUtf8Bytes(message, JsonSerializerOptions.Default);
        var header = new MessageHeader(type, serialized.Length);

        await this._connection.SendAsync(header.ToBytes());
        await this._connection.SendAsync(serialized);
        
        this._writeMutex.ReleaseMutex();
    }
    
    public bool IsConnected()
    {
        try
        {
            return !(this._connection.Poll(1, SelectMode.SelectRead) && this._connection.Available == 0);
        }
        catch (SocketException) { return false; }
    }
}