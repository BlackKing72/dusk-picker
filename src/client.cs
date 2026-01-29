using System.Net;
using System.Net.Sockets;

namespace Black.DuskPicker;

public class Client(IPAddress ip, int port = 8743) : IDisposable
{
    private readonly IPEndPoint endpoint = new(ip, port);
    private readonly Socket socket = new(
        AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp
    );

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing = false)
    {
        if (!disposing)
        {
            Console.WriteLine($"Server was not properly disposed.");
        }

        socket.Dispose();
    }

    public void SendCommand(ServerCommand command)
    {
        socket.Connect(endpoint);

        byte[] buffer = new byte[(byte)command];

        _ = socket.Send(buffer);
        socket.Close();
    }

    public async Task SendCommandAsync(ServerCommand command)
    {
        await socket.ConnectAsync(endpoint);

        byte[] buffer = [(byte)command];
        Console.WriteLine($"Sending: {buffer.Length} byte(s) - {buffer[0]}");

        _ = await socket.SendAsync(buffer);
        socket.Close();
    }
}
