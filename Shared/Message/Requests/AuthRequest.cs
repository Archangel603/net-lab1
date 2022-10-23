namespace Shared.Message.Requests;

public class AuthRequest : IRequest
{
    public string Username { get; set; }
    
    public string Password { get; set; }
}