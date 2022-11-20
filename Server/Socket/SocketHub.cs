using System.Collections.Concurrent;

namespace Server.Socket;

public class SocketHub
{
    private ConcurrentDictionary<Guid, SocketClient> _clients = new();

    public IReadOnlyDictionary<Guid, SocketClient> Clients => this._clients;

    public void AddClient(SocketClient client)
    {
        this._clients[client.Id] = client;
    }

    public void RemoveClient(Guid clientId)
    {
        this._clients.Remove(clientId, out _);
    }
    
    public async Task SendToAll(object message)
    {
        foreach (var (_, client) in this._clients)
        {
            try
            {
                await client.Connection.WriteMessage(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
    
    public async Task SendToClient(Guid clientId, object message)
    {
        await this._clients[clientId].Connection.WriteMessage(message);
    }
    
    public async Task SendToUser(Guid clientId, object message)
    {
        await Task.WhenAll(
            this._clients.Values
                .Where(x => x.User?.Id == clientId)
                .Select(x => x.Connection.WriteMessage(message))
        );
    }
}