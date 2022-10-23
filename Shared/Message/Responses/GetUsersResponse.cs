using Shared.Model;

namespace Shared.Message.Responses;

public class GetUsersResponse
{
    public List<UserInfo> Users { get; set; } = new();

    public GetUsersResponse()
    {
        
    }
    
    public GetUsersResponse(IEnumerable<UserInfo> users)
    {
        this.Users.AddRange(users);
    }
}