<div align="center">
<img width="128" height="128" alt="Dusk picker icon" src="https://github.com/user-attachments/assets/78b82181-39ca-43b4-89d7-fd282c0f19a9" />

<h1>Dusk Picker</h1>
<p>A simple desktop color picker tool inspired by the Powertoyz Color Picker.</p>
</div>

</br>

Dusk Picker is a simple desktop color picker tool that started as a weekend project.

I'm color blind and frequently used Powertoyz Color Picker to identify the color name. Since PowerToyz only works on Windows, I decided to create a similar tool forLinux.

It's not meant to be a full replacement, but rather a minimal picker with focus on usability.

</br>

> [!NOTE]
> Currently tested only on **Linux (Pop! OS 22.04)** with X11, so Ubuntu or similar distros should work too. Windows supported is planned.

</br>
<div align="center">
<img width="943" height="531" alt="preview" src="https://github.com/user-attachments/assets/5de09ccc-a5f3-4305-8d9d-e8f6e9a96dc2" />
</div>
</br>

## Features

- Pick colors from anywhere on the desktop.
- Display the color name.
- Copy the values as:
  - RGB (0-255).
  - RGB (0-1).
  - HEX.
 
</br>

## Libraries

- **[Shutter](https://github.com/rthomasv3/Shutter/tree/master/Shutter)** - To capture desktop screenshots.
- **[Raylib](https://github.com/raylib-cs/raylib-cs/tree/master/Raylib-cs)** - To create the window and rendering the UI.
- **[ImGui](https://github.com/HexaEngine/Hexa.NET.ImGui)** - To create the user interface.

Other mentions:
- [RlImGui-cs](https://github.com/HexaEngine/Hexa.NET.Raylib/tree/master/ExampleImGui) - A Raylib renderer for ImGui
- [ImThemes](https://github.com/Patitotective/ImThemes) - Custom themes for ImGui.
  - Comfortable Light Orange styleSouthCraftX
  - Comfortable Dark Cyan styleSouthCraftX

</br>

## Requirements

- .NET 10
- OpenGL 3.3

It uses **Shutter** under the hood, so the same requirements apply.
- **Windows**: Windows 7 or higher.
- **Linux**:
  - **X11**: **libX11** installed.
  - **Wayland**: **DBus** and **XDG Desktop Portal** support.
- [See full requirements.](https://github.com/rthomasv3/Shutter?tab=readme-ov-file#requirements)

</br>

## Usage

The app is split in two parts:
- A **server** that runs in the background.
- A **client** that sends commands to the server.

**1 - Start the server**

  ```sh
  dusk-picker --server
  ```

**2 - Open the picker**

```sh
dusk-picker --open
```

> [!IMPORTANT]
> The picker opens as a fullscreen window on top of the desktop. Press `Esc` to close it.

</br>

### Automating Usage

**On Pop!OS / Ubuntu**

The server can be started automatically at login using either **Startup Applications** or manually creating a systemd service.

A custom keyboard shortcut can be assigned using the **System Settings** to run the command that opens the picker window.

> [!NOTE]
> Make sure the shell has your profile loaded, in **zsh** is the `-l` or `--login` option. example: `zsh -l -c "command"`

</br>

### Keybinds

- `Left Click` - Pick a color.
- `Scroll Wheel`- Adjust zoom level.
- `Esc` - Closes the picker window.
- `F10` - Closes the picker window and shutdown the server.

</br>

### Commands

```sh
dusk-picker --help    # shows a help message.
dusk-picker --open    # open picker window.
dusk-picker --close   # close picker window.
dusk-picker --quit    # close picker window and shutdown the server.

# sets the port used by the server.
# make sure to change the port on both, server and client.
dusk-picker --port=1234
```

</br>

## Build

You can build it using:

```sh
dotnet build
dotnet publish
```

or

```
dotnet run -- --server
```

> [!NOTE]
> To pass arguments to `dotnet run` use a `--` (double dash) followed by the arguments.

</br>

## Cross-Compile

To cross-compile you must specify the target RID. See the RID catalog in https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids

```
dotnet build -r linux-x64
dotnet publish -r win-x64
```
