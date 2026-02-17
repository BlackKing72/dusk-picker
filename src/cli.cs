using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

namespace Black.DuskPicker;

public static class CLI
{
    public record class Result(
        bool IsServer = true,
        int Port = 8743,
        ServerCommand Command = default,
        PickerPalette Palette = PickerPalette.Xkcd
    );



    public class CustomHelpAction(HelpAction helpAction) : SynchronousCommandLineAction
    {
        private readonly HelpAction defaultHelpAction = helpAction;

        public override int Invoke(ParseResult parseResult)
        {
            const int UsageExitCode = 64;
            _ = defaultHelpAction.Invoke(parseResult);
            return UsageExitCode;
        }
    }

    public static int ParseArguments(Action<Result> onParseArguments, params string[] args)
    {
        Option<bool> serverOption = new("--server", "-s", "--daemon", "-d")
        {
            Description =
                "Starts the server instance, create a window and preload the assets. Use --open to also open the window",
        };
        Option<bool> showOption = new("--open", "-o")
        {
            Description = "Sends a show command that opens the window on the server instance.",
        };
        Option<bool> hideOption = new("--close", "-c")
        {
            Description = "Sends a hide command that close the window on the server instance.",
        };
        Option<bool> quitOption = new("--quit", "-q")
        {
            Description = "Shutdown the server instance, and closes the window.",
        };
        Option<int?> portOption = new("--port", "-P")
        {
            Description = "The port used to sending and receiving commands.",
            DefaultValueFactory = parseResult => null,
        };

        Option<PickerPalette?> paletteOption = new("--palette", "-p")
        {
            Description = "The color palette to use when getting color names",
            DefaultValueFactory = parseResult => PickerPalette.Xkcd,
        };

        RootCommand rootCommand = new(
            """
            Dusk Picker

            Shortcuts:
              Left Click - Selects a color
              Esc - Closes the window
              F10 - Shutdown the server.
            """
        );

        rootCommand.Options.Add(serverOption);
        rootCommand.Options.Add(portOption);
        rootCommand.Options.Add(showOption);
        rootCommand.Options.Add(hideOption);
        rootCommand.Options.Add(quitOption);
        rootCommand.Options.Add(paletteOption);

        for (int i = 0; i < rootCommand.Options.Count; i++)
        {
            if (rootCommand.Options[i] is HelpOption defaultHelpOption)
            {
                var defaultHelpAction = (HelpAction)defaultHelpOption.Action!;
                defaultHelpOption.Action = new CustomHelpAction(defaultHelpAction);
                break;
            }
        }

        rootCommand.SetAction(
            (parseResult) =>
            {
                var result = new Result();

                if (parseResult.GetValue(serverOption) is bool isServer)
                    result = result with { IsServer = isServer };

                if (parseResult.GetValue(portOption) is int port)
                    result = result with { Port = port };

                if (parseResult.GetValue(paletteOption) is PickerPalette palette)
                    result = result with { Palette = palette };

                var optionsToCommandMap = new Dictionary<Option<bool>, ServerCommand>()
                {
                    [showOption] = ServerCommand.Show,
                    [hideOption] = ServerCommand.Hide,
                    [quitOption] = ServerCommand.Quit,
                };

                foreach (var (option, command) in optionsToCommandMap)
                {
                    if (parseResult.GetValue(option) is bool value && value)
                    {
                        result = result with { Command = command };
                        break;
                    }
                }

                onParseArguments(result);
            }
        );

        ParseResult parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            foreach (var parseError in parseResult.Errors)
                Console.WriteLine(parseError);

            return -1;
        }

        return parseResult.Invoke();
    }
}
