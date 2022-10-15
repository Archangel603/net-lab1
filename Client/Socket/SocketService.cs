using System.Net;
using System.Net.Sockets;
using Shared.Socket;

namespace Client.Socket;

public class SocketService
{
    private SocketConnection _connection;

    public async Task Connect(string address, int port)
    {
        var addresses = await Dns.GetHostAddressesAsync(address);

        if (addresses.Length == 0)
        {
            throw new Exception("Host not found");
        }

        var socket = new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(addresses, port);

        this._connection = new SocketConnection(socket);
    }
}