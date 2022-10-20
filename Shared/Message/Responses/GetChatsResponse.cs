using Shared.Model;

namespace Shared.Message.Responses;

public class GetChatsResponse
{
    public List<ChatInfo> Chats { get; set; } = new();

    public GetChatsResponse()
    {
        
    }
    
    public GetChatsResponse(IEnumerable<ChatInfo> chats)
    {
        this.Chats.AddRange(chats);
    }
}