namespace Server.Model;

public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public List<Chat> Chats { get; set; }
}