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
    await RunAsServer(result);
}
else
{
    await RunAsClient(result);
}

static async Task RunAsClient(CLI.Result result)
{
    ServerCommand command = result.Command;
    int port = result.Port;

    Console.WriteLine($"Running as client...");
    Console.WriteLine($"  send commands {command} to server @{IPAddress.Loopback}:{port}");

    Client client = new(IPAddress.Loopback, port);
    await client.SendCommandAsync(command);
}

static async Task RunAsServer(CLI.Result result)
{
    ServerCommand command = result.Command;
    int port = result.Port;

    Server server = new(IPAddress.Loopback, port);
    Console.WriteLine($"Running as server...");
    Console.WriteLine($"  listening on {IPAddress.Loopback}:{port}");

    using Picker app = new(new PickerOptions()
    {
        Palette = result.Palette
    });

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
