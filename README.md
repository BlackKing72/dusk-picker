<div align="center">
<img width="128" height="128" alt="Dusk picker icon" src="https://github.com/user-attachments/assets/78b82181-39ca-43b4-89d7-fd282c0f19a9" />

<h1>Dusk Picker</h1>
<p>A simple desktop color picker tool inspired by the PowerToyz Color Picker.</p>
</div>

</br>

Dusk Picker is a simple desktop color picker tool that started as a weekend project.

I'm color blind and frequently used PowerToyz Color Picker to identify the color name. Since PowerToyz only works on Windows, I decided to create a similar tool for Linux.

It's not meant to be a full replacement, but rather a minimal picker with focus on usability.

</br>

> [!NOTE]
> Currently tested only on **Linux (Pop! OS 22.04)** with X11, so Ubuntu or similar distros should work too. Windows support is planned.

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

## Built With

- **[Shutter](https://github.com/rthomasv3/Shutter/tree/master/Shutter)** - To capture desktop screenshots.
- **[Raylib](https://github.com/raylib-cs/raylib-cs/tree/master/Raylib-cs)** - To create the window and rendering the UI.
- **[ImGui](https://github.com/HexaEngine/Hexa.NET.ImGui)** - To create the user interface.

Other mentions:
- [RlImGui-cs](https://github.com/HexaEngine/Hexa.NET.Raylib/tree/master/ExampleImGui) - A Raylib renderer for ImGui
- [ImThemes](https://github.com/Patitotective/ImThemes) - Custom themes for ImGui.
  - Comfortable Light Orange by SouthCraftX
  - Comfortable Dark Cyan by SouthCraftX

</br>

## Requirements

- .NET 10*
- OpenGL 3.3

It uses **Shutter** under the hood, so the same requirements apply.
- **Windows**: Windows 7 or higher.
- **Linux**:
  - **X11**: **libX11** installed.
  - **Wayland**: **DBus** and **XDG Desktop Portal** support.
- [See full requirements.](https://github.com/rthomasv3/Shutter?tab=readme-ov-file#requirements)

> [!NOTE] **.NET 10** is required if the app is built with `PublishAOT` disabled.

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

A custom keyboard shortcut can be assigned in **System Settings** to run the command that opens the picker window.

> [!NOTE]
> Depending on how you automate it, you may need to ensure the shell loads your profile, so the executable and .NET runtime can be located correctly. 
> In **zsh** use the `-l` or `--login` option. example: `zsh -l -c "command"`

</br>

### Keybinds

- `Left Click` - Pick a color.
- `Scroll Wheel` - Adjust zoom level.
- `Esc` - Closes the picker window.
- `F10` - Closes the picker window and shutdown the server.

</br>

### Commands

```sh
dusk-picker --help    # show help message.
dusk-picker --open    # open picker window.
dusk-picker --close   # close picker window.
dusk-picker --quit    # close picker window and shutdown the server.
dusk-picker --palette # sets the palette used to get color names <Web|Toyz|Xkcd>.

# sets the port used by the server.
# make sure to change the port on both, server and client.
dusk-picker --port=1234
```

</br>

## Build

### Prerequisites

This project has `PublishAOT` enabled by default, so you will need to install the prerequisites depending on the platform you are building on.

**Windows**: **Visual Studio 2022+** or **Build tools for Visual Studio** with **Desktop development with C++ workload** installed.
**Linux (Ubuntu)**: Ubuntu 18.04+ with **clang** and **zlib1g-dev** installed.

For more detailed and other platforms (Alpine, Fedora, MacOS) prerequisites see [Native AOT deployment Prerequisites](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=linux-ubuntu%2Cnet8#prerequisites)

### Build

After the prerequisites are installed building can be done simply by:

```sh
dotnet build
```

or publish to generate a native binary:
```sh
dotnet publish
```

The app can be run directly using:

```sh
dotnet run -- --server --open
```

> [!NOTE]
> To pass arguments to `dotnet run` use a `--` (double dash) followed by the arguments.

</br>

## Publish and Cross-Compile

With `PublishAOT` enabled, the publish command will fail if you are not on the target platform, to cross-compile you must disable AOT compilation on the `.csproj`, this will create a binary that is framework dependent, requiring the runtime to be installed in the target machine.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Comment this line or set it to false -->
    <PublishAot>true</PublishAot>
  </PropertyGroup>
</Project>
```

Then specify the target RID of the platform. See the RID catalog in https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids

```sh
dotnet build -r linux-x64
# or
dotnet publish -r win-x64
```
