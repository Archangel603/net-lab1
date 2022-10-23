namespace Shared.Model;

public class MessageInfo
{
    public UserInfo Sender { get; set; }
    
    public string Text { get; set; }
    
    public DateTime CreatedAt { get; set; }
}