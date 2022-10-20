using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.MessageHandlers;

public class GetChatsRequestHandler : IMessageHandler
{
    private readonly ChatService _chatService;

    public GetChatsRequestHandler(ChatService chatService)
    {
        this._chatService = chatService;
    }
    
    public async Task HandleMessage(Message message, SocketClient client)
    {
        var request = message.Body.Read<GetChatsRequest>();

        if (!await client.TryAuthenticate(request))
            return;

        var chats = await this._chatService.GetChats(client.User.Id);

        await client.Connection.WriteMessage(MessageType.GetChatsResponse, new GetChatsResponse(
            chats.Select(c => new ChatInfo
            {
                Id = c.Id,
                Type = c.Type,
                Users = c.Users.Select(u => new UserInfo(u.Id, u.Username)).ToList()
            })    
        ));
    }
}