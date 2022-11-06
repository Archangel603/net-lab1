using Server.Services;
using Server.Socket;
using Shared.Message.Events;
using Shared.Message.Requests;

namespace Server.RequestHandlers;

public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest>
{
    private readonly UserService _userService;
    private readonly EventBus _eventBus;

    public DeleteUserRequestHandler(UserService userService, EventBus eventBus)
    {
        this._userService = userService;
        this._eventBus = eventBus;
    }

    public async Task Handle(DeleteUserRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request, true))
            return;

        await this._userService.DeleteUser(request.UserId);

        await this._eventBus.PublishEvent(new UserDeletedEvent(request.UserId));
    }
}