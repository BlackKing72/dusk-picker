#pragma warning disable CA1822

using Raylib_cs;

namespace Black.DuskPicker;

public class Application : IDisposable
{
    protected readonly LayerManager layerManager;
    protected readonly GuiManager guiManager;

    public Application()
    {
        Raylib.SetConfigFlags(ConfigFlags.UndecoratedWindow);
        Raylib.SetConfigFlags(ConfigFlags.TransparentWindow);
        Raylib.SetConfigFlags(ConfigFlags.HiddenWindow);
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow);

        Raylib.InitWindow(960, 540, "Color Picker");

        var iconData = Embedded.ReadBytes("assets/dusk.png");
        var iconImage = Raylib.LoadImageFromMemory(".png", iconData);
        Raylib.SetWindowIcon(iconImage);

        Raylib.SetExitKey(KeyboardKey.F10);
        Raylib.SetWindowState(ConfigFlags.BorderlessWindowMode);
        Raylib.SetWindowPosition(0, 0);
        Raylib.EnableEventWaiting();

        layerManager = new LayerManager();
        guiManager = new GuiManager(false);

        EventSystem.OnEvent += OnEvent;
        EventSystem.OnEvent += layerManager.OnEvent;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing = false)
    {
        EventSystem.OnEvent -= layerManager.OnEvent;

        guiManager.Dispose();
        layerManager.Dispose();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            OnUpdate();

            if (Raylib.IsKeyPressed(KeyboardKey.B))
            {
                Raylib.ToggleBorderlessWindowed();

                if (Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode))
                {
                    Raylib.SetWindowPosition(0, 0);
                }
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(0, 0, 0, 0));

            // Update and render layers
            layerManager.OnUpdate();
            layerManager.OnRender();

            guiManager.BeginGui();
            layerManager.OnGui();
            guiManager.EndGui();

            Raylib.EndDrawing();
        }
    }

    public void ShowWindow()
    {
        // The event is called before the window opens, so the listeners can consume
        // the event before the window draws the first frame.
        EventSystem.RaiseEvent(new WindowOpenEvent());
        Raylib.ClearWindowState(ConfigFlags.HiddenWindow);

        // Disable the wait for events, because imgui needed to be updated every frame.
        Raylib.DisableEventWaiting();

        // Reset the window position, because the window keeps moving below the PopOS top bar.
        Raylib.SetWindowPosition(0, 0);
    }

    public void HideWindow()
    {
        // When the window is hidden, it's fine to wait for events.
        Raylib.EnableEventWaiting();

        EventSystem.RaiseEvent(new WindowCloseEvent());
        Raylib.SetWindowState(ConfigFlags.HiddenWindow);
    }

    public void CloseWindow()
    {
        Raylib.CloseWindow();
    }

    protected virtual void OnEvent(Event evt) { }

    protected virtual void OnUpdate() { }
}

#pragma warning restore CA1822
