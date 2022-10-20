namespace Shared.Message.Requests;

public class LeaveChatRequest : AuthenticatedRequest
{
    public Guid ChatId { get; set; }
}