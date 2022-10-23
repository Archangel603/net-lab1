using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.RequestHandlers;

public class GetChatsRequestHandler : IRequestHandler<GetChatsRequest>
{
    private readonly ChatService _chatService;

    public GetChatsRequestHandler(ChatService chatService)
    {
        this._chatService = chatService;
    }
    
    public async Task Handle(GetChatsRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
            return;

        var chats = await this._chatService.GetChats(client.User.Id);

        await client.Connection.WriteMessage(new GetChatsResponse(
            chats.Select(c => new ChatInfo
            {
                Id = c.Id,
                Type = c.Type,
                Users = c.Users.Select(u => new UserInfo(u.Id, u.Username)).ToList()
            })    
        ));
    }
}