namespace Shared.Message.Requests;

public class RegisterRequest : IRequest
{
    public string Username { get; set; }
    
    public string Password { get; set; }
}