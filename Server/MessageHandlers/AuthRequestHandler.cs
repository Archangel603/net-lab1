using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Events;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.MessageHandlers;

public class AuthRequestHandler : IMessageHandler
{
    private readonly UserService _userService;
    private readonly EventBus _eventBus;
    
    public AuthRequestHandler(UserService userService, EventBus eventBus)
    {
        this._userService = userService;
        this._eventBus = eventBus;
    }

    public async Task HandleMessage(Message message, SocketClient client)
    {
        var form = message.Body.Read<AuthRequest>();

        if (String.IsNullOrWhiteSpace(form.Username))
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Username is required"));
            return;
        }
        
        if (String.IsNullOrWhiteSpace(form.Password))
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Password is required"));
            return;
        }

        var user = await this._userService.Find(form.Username);

        if (user is null)
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("User not found"));
            return;
        }

        if (!this._userService.CheckPassword(user, form.Password))
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Invalid password"));
            return;
        }

        client.Authorize(Guid.NewGuid(), new UserInfo(user.Id, user.Username));

        await client.Connection.WriteMessage(MessageType.AuthResponse, new AuthResponse
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