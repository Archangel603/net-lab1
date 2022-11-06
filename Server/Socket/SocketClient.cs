using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;
using Shared.Socket;

namespace Server.Socket;

public class SocketClient
{
    public delegate SocketClient Factory(Guid id, SocketConnection connection);
    
    public SocketConnection Connection { get; }
 
    public Guid SessionKey { get; private set; }
    
    public Guid Id { get; }
    
    public UserInfo? User { get; private set; }
    
    public SocketClient(Guid id, SocketConnection connection)
    {
        this.Id = id;
        this.Connection = connection;
    }

    public void Authorize(Guid sessionKey, UserInfo userInfo)
    {
        this.SessionKey = sessionKey;
        this.User = userInfo;
    }

    public async Task<bool> TryAuthenticate(AuthenticatedRequest request, bool mustBeAdmin = false)
    {
        if (request.SessionKey != SessionKey)
        {
            await this.Connection.WriteMessage(new ErrorResponse("Unauthorized"));
            return false;
        }
        
        if (mustBeAdmin && !User.IsAdmin)
        {
            await this.Connection.WriteMessage(new ErrorResponse("Forbidden"));
            return false;
        }
        
        return true;
    }
}