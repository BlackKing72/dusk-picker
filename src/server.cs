using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Black.DuskPicker;

public enum ServerCommand : byte
{
    // None = 0,
    // Ping = 1,
    Show = 2,
    Hide = 3,
    Quit = 4,
}

public class Server(IPAddress ip, int port = 8743) : IDisposable
{
    public event Action<ServerCommand>? CommandReceived;

    private readonly IPEndPoint endpoint = new(ip, port);
    private readonly Socket socket = new(
        AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp
    );

    private bool shouldClose;

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

        StopListen();
        socket.Dispose();
    }

    public void Listen()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                socket.Bind(endpoint);
                socket.Listen(5);

                while (!shouldClose)
                {
                    Socket listener = socket.Accept();

                    byte[] buffer = new byte[1];
                    int readSize = await listener.ReceiveAsync(buffer, SocketFlags.None);
                    byte value = buffer[0];

                    if (!TryGetServerCommand(value, out ServerCommand serverCommand))
                    {
                        Console.WriteLine(
                            $"Server Received: Unknown Command '{value}'. Skipping..."
                        );
                        continue;
                    }

                    CommandReceived?.Invoke(serverCommand);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server Error: {ex.ToString()}");
            }
        });
    }

    public void StopListen()
    {
        shouldClose = true;
    }

    private static bool TryGetServerCommand(
        object value,
        [NotNullWhen(true)] out ServerCommand command
    )
    {
        bool isServerCommand = Enum.IsDefined(typeof(ServerCommand), value);
        command = isServerCommand ? (ServerCommand)value : default;

        return isServerCommand;
    }
}
