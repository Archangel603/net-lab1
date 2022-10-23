namespace Shared.Message.Requests;

public class SendMessageRequest : AuthenticatedRequest
{
    public Guid ChatId { get; set; }
    
    public string Text { get; set; }
}