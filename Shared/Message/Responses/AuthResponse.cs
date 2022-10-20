using Shared.Model;

namespace Shared.Message.Responses;

public class AuthResponse
{
    public Guid SessionKey { get; set; }
    
    public UserInfo User { get; set; }
}