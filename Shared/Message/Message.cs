using System.Text.Json;

namespace Shared.Message;

public enum MessageType
{
    // Requests
    AuthRequest, RegisterRequest,
    GetMessagesRequest, SendMessageRequest,
    GetChatsRequest, CreateChatRequest, JoinChatRequest, LeaveChatRequest,
    
    // Responses
    AuthResponse, RegisterResponse,
    GetMessagesResponse, SendMessageResponse,
    GetChatsResponse, CreateChatResponse, JoinChatResponse, LeaveChatResponse,
    
    // Events
    MessageReceived,
    UserJoinedChat, UserLeftChat,
    UserOnline, UserOffline,
    
    Error
}

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
    public MessageType Type { get; }
    public long Length { get; }

    public MessageHeader(MessageType type, long length)
    {
        this.Type = type;
        this.Length = length;
    }

    public byte[] ToBytes()
    {
        var length = BitConverter.GetBytes(this.Length);

        return length.Prepend((byte)this.Type).ToArray();
    }
    
    public static MessageHeader FromBytes(byte[] bytes)
    {
        return new MessageHeader((MessageType)bytes[0], BitConverter.ToInt64(bytes[1..]));
    }
}

public class MessageBodyReader
{
    private readonly Memory<byte> _buffer;

    public MessageBodyReader(Memory<byte> buffer)
    {
        this._buffer = buffer;
    }

    public T Read<T>()
    {
        return JsonSerializer.Deserialize<T>(this._buffer.Span, JsonSerializerOptions.Default);
    }
}