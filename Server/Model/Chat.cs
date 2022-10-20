using Shared.Model;

namespace Server.Model;

public class Chat
{
    public Guid Id { get; set; }
    
    public ChatType Type { get; set; }
    
    public List<User> Users { get; set; }
}