namespace Shared.Message.Requests;

public abstract class AuthenticatedRequest : IRequest
{
    public Guid SessionKey { get; set; }
}