using System.Net;
using Black.DuskPicker;

// to lauch using pop os shortcuts
// `zsh -l -c "/path/to/executable"
//   -l -> login to shell (loads zshrc/zprofile)
//   -c -> command to execute

CLI.Result result = new();
int exitCode = CLI.ParseArguments((x) => result = x, args);
if (exitCode != 0)
{
    Environment.Exit(exitCode);
}

bool isServer = result.IsServer;

if (isServer)
{
    await RunAsServer(result.Command, result.Port);
}
else
{
    await RunAsClient(result.Command, result.Port);
}

static async Task RunAsClient(ServerCommand command, int port = 8743)
{
    Console.WriteLine($"Running as client...");
    Console.WriteLine($"  send commands {command} to server @{IPAddress.Loopback}:{port}");

    Client client = new(IPAddress.Loopback, port);
    await client.SendCommandAsync(command);
}

static async Task RunAsServer(ServerCommand command, int port = 8743)
{
    Server server = new(IPAddress.Loopback, port);
    Console.WriteLine($"Running as server...");
    Console.WriteLine($"  listening on {IPAddress.Loopback}:{port}");

    using Picker app = new();

    server.CommandReceived += (command) =>
    {
        Console.WriteLine($"Received command {command}");
        switch (command)
        {
            case ServerCommand.Quit:
                app.CloseWindow();
                break;

            case ServerCommand.Show:
                app.ShowWindow();
                break;

            case ServerCommand.Hide:
                app.HideWindow();
                break;

            default:
                throw new ArgumentException($"Unknown command {command}.");
        }
    };

    server.Listen();

    if (command is ServerCommand.Show)
    {
        app.ShowWindow();
    }

    app.Run();
}
