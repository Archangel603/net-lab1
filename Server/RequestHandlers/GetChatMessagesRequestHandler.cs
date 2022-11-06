using Server.Extensions;
using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.RequestHandlers;

public class GetChatMessagesRequestHandler : IRequestHandler<GetChatMessagesRequest>
{
    private readonly ChatService _chatService;

    public GetChatMessagesRequestHandler(ChatService chatService)
    {
        this._chatService = chatService;
    }

    public async Task Handle(GetChatMessagesRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
            return;

        var messages = await this._chatService.GetMessages(request.ChatId);

        await client.Connection.WriteMessage(new GetChatMessagesResponse(request.ChatId, messages
            .Select(m => new MessageInfo
        {
            Sender = m.Sender.ToUserInfo(),
            Text = m.Text,
            CreatedAt = m.CreatedAt
        })));
    }
}