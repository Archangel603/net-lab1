using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Events;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.MessageHandlers;

public class AuthRequestHandler : IRequestHandler<AuthRequest>
{
    private readonly UserService _userService;
    private readonly EventBus _eventBus;
    
    public AuthRequestHandler(UserService userService, EventBus eventBus)
    {
        this._userService = userService;
        this._eventBus = eventBus;
    }

    public async Task Handle(AuthRequest request, SocketClient client)
    {
        if (String.IsNullOrWhiteSpace(request.Username))
        {
            await client.Connection.WriteMessage(new ErrorResponse("Username is required"));
            return;
        }
        
        if (String.IsNullOrWhiteSpace(request.Password))
        {
            await client.Connection.WriteMessage(new ErrorResponse("Password is required"));
            return;
        }

        var user = await this._userService.Find(request.Username);

        if (user is null)
        {
            await client.Connection.WriteMessage(new ErrorResponse("User not found"));
            return;
        }

        if (!this._userService.CheckPassword(user, request.Password))
        {
            await client.Connection.WriteMessage(new ErrorResponse("Invalid password"));
            return;
        }

        client.Authorize(Guid.NewGuid(), new UserInfo(user.Id, user.Username));

        await client.Connection.WriteMessage(new AuthResponse
        {
            SessionKey = client.SessionKey,
            User = client.User
        });

        await this._eventBus.PublishEvent(new UserOnlineEvent
        {
            User = client.User
        });
    }
}