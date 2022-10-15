namespace Server.Model;

public enum ChatType
{
    Public, Private
}

public class Chat
{
    public Guid Id { get; set; }
    
    public ChatType Type { get; set; }
    
    public List<User> Users { get; set; }
}