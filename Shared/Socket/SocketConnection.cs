using System.Net.Sockets;
using System.Text.Json;
using Shared.Message;

namespace Shared.Socket;

public class SocketConnection
{
    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
    private readonly System.Net.Sockets.Socket _connection;

    public SocketConnection(System.Net.Sockets.Socket connection)
    {
        this._connection = connection;
    }

    public async Task<Message.Message> ReadMessage()
    {
        await this._readSemaphore.WaitAsync();

        try
        {
            var bytes = new byte[9];
            var received = await this._connection.ReceiveAsync(bytes);

            if (bytes.Length != received)
            {
                throw new Exception("Unable to read header");
            }

            var header = MessageHeader.FromBytes(bytes);
            var buffer = new Memory<byte>(new byte[header.Length]);

            await this._connection.ReceiveAsync(buffer);
            
            Console.WriteLine($"Received message of type {header.Type}");
            
            return new Message.Message(header, new MessageBodyReader(buffer));
        }
        finally
        {
            this._readSemaphore.Release();
        }
    }
    
    public async Task WriteMessage<T>(MessageType type, T message)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        await this._writeSemaphore.WaitAsync(cts.Token);

        try
        {
            var serialized = JsonSerializer.SerializeToUtf8Bytes(message, JsonSerializerOptions.Default);
            var header = new MessageHeader(type, serialized.Length);

            await this._connection.SendAsync(header.ToBytes());
            await this._connection.SendAsync(serialized);
            Console.WriteLine($"Sent message of type {type}");
        }
        finally
        {
            this._writeSemaphore.Release();
        }
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