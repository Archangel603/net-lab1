using Server.Extensions;
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

        var chatInfos = new List<ChatInfo>(chats.Count);

        foreach (var c in chats)
        {
            chatInfos.Add(new ChatInfo
            {
                Id = c.Id,
                Type = c.Type,
                Users = c.Users.Select(u => u.ToUserInfo()).ToList(),
                LastMessage = (await this._chatService.GetLastMessage(c.Id))?.ToMessageInfo()
            });
        }
        
        await client.Connection.WriteMessage(new GetChatsResponse(chatInfos));
    }
}