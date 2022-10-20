namespace Shared.Model;

public class ChatInfo
{
    public Guid Id { get; set; }
    
    public ChatType Type { get; set; }
    
    public List<UserInfo> Users { get; set; }
}