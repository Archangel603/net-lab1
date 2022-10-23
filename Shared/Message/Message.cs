using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Shared.Message;

public class Message
{
    public MessageHeader Header { get; }
    
    public MessageBodyReader Body { get; }

    public Message(MessageHeader header, MessageBodyReader body)
    {
        this.Header = header;
        this.Body = body;
    }
}

public class MessageHeader
{
    public int TypeLength { get; }
    public long BodyLength { get; }

    public MessageHeader(int typeLength, long bodyLength)
    {
        this.TypeLength = typeLength;
        this.BodyLength = bodyLength;
    }

    public byte[] ToBytes()
    {
        var typeLength = BitConverter.GetBytes(this.TypeLength);
        var bodyLength = BitConverter.GetBytes(this.BodyLength);

        return typeLength.Concat(bodyLength).ToArray();
    }
    
    public static MessageHeader FromBytes(byte[] bytes)
    {
        var typeLength = BitConverter.ToInt32(bytes[0..4]);
        var bodyLength = BitConverter.ToInt32(bytes[4..]);
        
        return new MessageHeader(typeLength, bodyLength);
    }
}

public class MessageBodyReader
{
    private readonly MessageHeader _header;
    private readonly Memory<byte> _buffer;

    public MessageBodyReader(MessageHeader header, Memory<byte> buffer)
    {
        this._buffer = buffer;
        this._header = header;
    }

    public string GetBodyTypeName()
    {
        return Encoding.UTF8.GetString(this._buffer.Span[0..this._header.TypeLength]);
    }
    
    public Type GetBodyType()
    {
        return Assembly.GetExecutingAssembly().GetType(GetBodyTypeName());
    }
    
    public object Read()
    {
        return JsonSerializer.Deserialize(this._buffer.Span[this._header.TypeLength..], this.GetBodyType(), JsonSerializerOptions.Default);
    }
}