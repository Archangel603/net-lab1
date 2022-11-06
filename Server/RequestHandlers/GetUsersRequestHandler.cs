using Server.Extensions;
using Server.Services;
using Server.Socket;
using Shared.Message.Requests;
using Shared.Message.Responses;

namespace Server.RequestHandlers;

public class GetUsersRequestHandler : IRequestHandler<GetUsersRequest>
{
    private readonly UserService _userService;
    private readonly SocketHub _socketHub;

    public GetUsersRequestHandler(UserService userService, SocketHub socketHub)
    {
        this._userService = userService;
        this._socketHub = socketHub;
    }

    public async Task Handle(GetUsersRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
        {
            return;
        }

        var users = await this._userService.GetList();

        await client.Connection.WriteMessage(new GetUsersResponse(
            users.Select(u => u.ToUserInfo(this._socketHub))
        ));
    }
}