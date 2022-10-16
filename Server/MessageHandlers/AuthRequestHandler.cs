using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Server.Db;
using Server.Socket;
using Shared.Message;
using Shared.Message.Responses;

namespace Server.MessageHandlers;

public class AuthRequestHandler : IMessageHandler
{
    private readonly ServerDbContext _db;
    
    public AuthRequestHandler(ServerDbContext db)
    {
        this._db = db;
    }

    public async Task HandleMessage(Message message, SocketClient client)
    {
        var authMessage = message.Body.Read<AuthRequest>();

        if (String.IsNullOrWhiteSpace(authMessage.Username))
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Username is required"));
            return;
        }
        
        if (String.IsNullOrWhiteSpace(authMessage.Password))
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Password is required"));
            return;
        }

        var user = await this._db.Users.FirstOrDefaultAsync(u => u.Username == authMessage.Username);

        if (user is null)
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("User not found"));
            return;
        }

        var passwordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(authMessage.Password)));

        if (passwordHash != user.PasswordHash)
        {
            await client.Connection.WriteMessage(MessageType.Error, new ErrorResponse("Invalid password"));
            return;
        }

        var sessionKey = Guid.NewGuid();
        client.SessionKey = sessionKey;

        await client.Connection.WriteMessage(MessageType.AuthResponse, new AuthResponse
        {
            SessionKey = sessionKey
        });
    }
}