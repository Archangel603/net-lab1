using Server.Services;
using Server.Socket;
using Shared.Message.Events;
using Shared.Message.Requests;
using Shared.Model;

namespace Server.RequestHandlers;

public class SendMessageRequestHandler : IRequestHandler<SendMessageRequest>
{
    private readonly ChatService _chatService;
    private readonly EventBus _eventBus;

    public SendMessageRequestHandler(ChatService chatService, EventBus eventBus)
    {
        this._chatService = chatService;
        this._eventBus = eventBus;
    }

    public async Task Handle(SendMessageRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
            return;

        var chat = await this._chatService.Find(request.ChatId);
        var message = await this._chatService.AddMessage(chat.Id, client.User.Id, request.Text);

        await this._eventBus.PublishEvent(new UserSentMessageEvent
        {
            ChatId = chat.Id,
            Message = new MessageInfo
            {
                Sender = client.User,
                Text = message.Text,
                CreatedAt = message.CreatedAt
            }
        });
    }
}