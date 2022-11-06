using Server.Model;
using Server.Socket;
using Shared.Model;

namespace Server.Extensions;

public static class UserExtensions
{
    public static UserInfo ToUserInfo(this User? user)
    {
        return user is null ? UserInfo.DeletedUserInfo : new UserInfo(
            user.Id, user.Username, user.IsAdmin    
        );
    }
    
    public static UserInfo ToUserInfo(this User? user, SocketHub hub)
    {
        return user is null ? UserInfo.DeletedUserInfo : new UserInfo(
            user.Id, user.Username, user.IsAdmin, 
            hub.Clients.Any(x => x.Value.User?.Id == user.Id)
        );
    }
}