using Shared.Model;

namespace Shared.Message.Responses;

public class GetChatMessagesResponse
{
    public Guid ChatId { get; set; }
    
    public List<MessageInfo> Messages { get; set; } = new();

    public GetChatMessagesResponse()
    {
    }

    public GetChatMessagesResponse(Guid chatId, IEnumerable<MessageInfo> messages)
    {
        this.ChatId = chatId;
        this.Messages.AddRange(messages);
    }
}