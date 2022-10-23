namespace Shared.Message.Requests;

public class CreatePersonalChatRequest : AuthenticatedRequest
{
    public Guid TargetUserId { get; set; }

    public CreatePersonalChatRequest()
    {
        
    }

    public CreatePersonalChatRequest(Guid targetUserId)
    {
        this.TargetUserId = targetUserId;
    }
}