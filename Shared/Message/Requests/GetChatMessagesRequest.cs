namespace Shared.Message.Requests;

public class GetChatMessagesRequest : AuthenticatedRequest
{
    public Guid ChatId { get; set; }

    public GetChatMessagesRequest()
    {
        
    }

    public GetChatMessagesRequest(Guid chatId)
    {
        this.ChatId = chatId;
    }
}