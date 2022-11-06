namespace Shared.Model;

public class UserInfo
{
    public static UserInfo DeletedUserInfo => new(Guid.Empty, "Deleted", false, false);
    
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public bool Online { get; set; }

    public UserInfo()
    {
        
    }

    public UserInfo(Guid id, string name, bool isAdmin, bool online = false)
    {
        this.Id = id;
        this.Name = name;
        this.IsAdmin = isAdmin;
        this.Online = online;
    }
}