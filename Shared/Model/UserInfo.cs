namespace Shared.Model;

public class UserInfo
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public UserInfo()
    {
        
    }

    public UserInfo(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}