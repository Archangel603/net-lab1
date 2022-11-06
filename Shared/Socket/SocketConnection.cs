using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Shared.Message;

namespace Shared.Socket;

public class SocketConnection : IDisposable
{
    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
    private readonly System.Net.Sockets.Socket _socket;

    public SocketConnection(System.Net.Sockets.Socket socket)
    {
        this._socket = socket;
    }

    public async Task<Message.Message> ReadMessage()
    {
        await this._readSemaphore.WaitAsync();

        try
        {
            var bytes = new byte[12];
            var received = await this._socket.ReceiveAsync(bytes);

            if (bytes.Length != received)
            {
                throw new Exception("Unable to read header");
            }

            var header = MessageHeader.FromBytes(bytes);
            var buffer = new Memory<byte>(new byte[header.TypeLength + header.BodyLength]);

            await this._socket.ReceiveAsync(buffer);

            var bodyReader = new MessageBodyReader(header, buffer);
            
            Console.WriteLine($"Received message of type {bodyReader.GetBodyTypeName()}");
            
            return new Message.Message(header, bodyReader);
        }
        finally
        {
            this._readSemaphore.Release();
        }
    }
    
    public async Task WriteMessage(object message)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        
        await this._writeSemaphore.WaitAsync(cts.Token);

        try
        {
            var typeName = message.GetType().FullName;
            var serialized = JsonSerializer.SerializeToUtf8Bytes(message, JsonSerializerOptions.Default);
            var typeNameBytes = Encoding.UTF8.GetBytes(typeName);
            var header = new MessageHeader(typeNameBytes.Length, serialized.Length);
            
            await this._socket.SendAsync(header.ToBytes());
            await this._socket.SendAsync(typeNameBytes.Concat(serialized).ToArray());
            Console.WriteLine($"Sent message of type {typeName}");
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
            return !(this._socket.Poll(1, SelectMode.SelectRead) && this._socket.Available == 0);
        }
        catch (SocketException) { return false; }
    }

    public void Dispose()
    {
        this._socket.Dispose();
        this._readSemaphore.Dispose();
        this._writeSemaphore.Dispose();
    }
}