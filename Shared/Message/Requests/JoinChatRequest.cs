namespace Shared.Message.Requests;

public class JoinChatRequest : AuthenticatedRequest
{
    public Guid ChatId { get; set; }
}