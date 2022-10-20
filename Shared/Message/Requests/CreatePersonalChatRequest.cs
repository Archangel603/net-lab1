namespace Shared.Message;

public class CreatePersonalChatRequest : AuthenticatedRequest
{
    public Guid TargetUserId { get; set; }
}