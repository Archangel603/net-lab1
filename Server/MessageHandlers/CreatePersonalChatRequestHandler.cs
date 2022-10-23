using Server.Services;
using Server.Socket;
using Shared.Message;
using Shared.Message.Events;
using Shared.Message.Requests;
using Shared.Model;

namespace Server.MessageHandlers;

public class CreatePersonalChatRequestHandler : IRequestHandler<CreatePersonalChatRequest>
{
    private readonly ChatService _chatService;
    private readonly EventBus _eventBus;
    private readonly UserService _userService;
    
    public CreatePersonalChatRequestHandler(ChatService chatService, EventBus eventBus, UserService userService)
    {
        this._chatService = chatService;
        this._eventBus = eventBus;
        this._userService = userService;
    }

    public async Task Handle(CreatePersonalChatRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
            return;

        var chat = await this._chatService.CreatePrivateChat(client.User.Id, request.TargetUserId);
        var targetUser = await this._userService.Find(request.TargetUserId);
        
        await this._eventBus.PublishEvent(new UserJoinedChatEvent(chat.Id, client.User));
        await this._eventBus.PublishEvent(new UserJoinedChatEvent(chat.Id, new UserInfo
        {
            Id = targetUser.Id,
            Name = targetUser.Username
        }));
    }
}