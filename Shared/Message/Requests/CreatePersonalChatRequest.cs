namespace Shared.Message.Requests;

public class CreatePersonalChatRequest : AuthenticatedRequest
{
    public Guid TargetUserId { get; set; }
}