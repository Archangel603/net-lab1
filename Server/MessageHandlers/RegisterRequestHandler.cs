using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Responses;

namespace Server.MessageHandlers;

public class RegisterRequestHandler : IMessageHandler
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;

    public RegisterRequestHandler(UserService userService, ChatService chatService)
    {
        this._userService = userService;
        this._chatService = chatService;
    }

    public async Task HandleMessage(Message message, SocketClient client)
    {
        var form = message.Body.Read<RegisterRequest>();

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

        if (user is not null)
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("User with this username already exists"));
            return;
        }

        user = await this._userService.RegisterUser(form.Username, form.Password);
        
        await client.Connection.WriteMessage(MessageType.RegisterResponse, new {});
        await this._chatService.AddUserToPublicChat(user);
    }
}