namespace Shared.Message.Requests;

public class DeleteUserRequest : AuthenticatedRequest
{
    public Guid UserId { get; set; }

    public DeleteUserRequest()
    {
    }

    public DeleteUserRequest(Guid userId)
    {
        this.UserId = userId;
    }
}