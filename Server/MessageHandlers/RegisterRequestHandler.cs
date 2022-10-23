using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Requests;
using Shared.Message.Responses;

namespace Server.MessageHandlers;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest>
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;

    public RegisterRequestHandler(UserService userService, ChatService chatService)
    {
        this._userService = userService;
        this._chatService = chatService;
    }

    public async Task Handle(RegisterRequest request, SocketClient client)
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

        if (user is not null)
        {
            await client.Connection.WriteMessage(new ErrorResponse("User with this username already exists"));
            return;
        }

        user = await this._userService.RegisterUser(request.Username, request.Password);
        
        await client.Connection.WriteMessage(new {});
        await this._chatService.AddUserToPublicChat(user);
    }
}