using Server.Services;
using Server.Socket;
using Shared.Message.Requests;
using Shared.Message.Responses;
using Shared.Model;

namespace Server.RequestHandlers;

public class GetUsersRequestHandler : IRequestHandler<GetUsersRequest>
{
    private readonly UserService _userService;

    public GetUsersRequestHandler(UserService userService)
    {
        this._userService = userService;
    }

    public async Task Handle(GetUsersRequest request, SocketClient client)
    {
        if (!await client.TryAuthenticate(request))
        {
            return;
        }

        var users = await this._userService.GetList();

        await client.Connection.WriteMessage(new GetUsersResponse(
            users.Select(u => new UserInfo(u.Id, u.Username))
        ));
    }
}