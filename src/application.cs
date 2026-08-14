#pragma warning disable CA1822      // CA1822: Mark members as static
#pragma warning disable IDE0130     // IDE0130: Namespace does not match folder structure

using System.Diagnostics;
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

        // Todo: Fix topmost don't work on multi-screen setups
        // On PopOS the window still behind the top bar, this don't happen
        // when I have a single monitor connected.
        // This makes sure the window is the topmost window (i.e: over pip and overlays windows)
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow);

#if DEBUG
        // This also makes it harder to debug the app, so disable it when in debugging.
        Raylib.ClearWindowState(ConfigFlags.TopmostWindow);

        // Enable all logs in debug builds.
        Raylib.SetTraceLogLevel(TraceLogLevel.All);
#endif

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        Raylib.InitWindow(screenWidth, screenHeight, "Dusk Picker");

        // This has to run after the window is created, otherwise the second
        // monitor is not listed as connected.
        var workArea = RLMonitors.GetTotalWorkArea();
        Raylib.SetWindowPosition(workArea.Position);
        Raylib.SetWindowSize(workArea.Size);

        var iconData = Embedded.ReadBytes("assets/dusk.png");
        var iconImage = Raylib.LoadImageFromMemory(".png", iconData);
        Raylib.SetWindowIcon(iconImage);

        Raylib.SetExitKey(KeyboardKey.F10);
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(0));
        Raylib.EnableEventWaiting();

        layerManager = new LayerManager();
        guiManager = new GuiManager(false);

        EventSystem.OnEvent += OnEventInternal;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing = false)
    {
        EventSystem.OnEvent -= OnEventInternal;

        guiManager.Dispose();
        layerManager.Dispose();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            EventSystem.OnUpdate();

            if (Raylib.IsWindowHidden())
            {
                // EndDrawing() needs to be called, whitout this the process never sleeps.
                Raylib.EndDrawing();
                continue;
            }

            OnUpdate();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(0, 0, 0, 0));

            // Update and render layers
            layerManager.OnUpdate();
            layerManager.OnRender();

            guiManager.BeginGui();
            layerManager.OnGui();
            guiManager.EndGui();

#if DEBUG
            Raylib.DrawRectangle(3, 3, 96, 24, Color.Black with { A = 200 });
            Raylib.DrawFPS(5, 5);
#endif

            Raylib.EndDrawing();
        }
    }

    protected virtual void OnEvent(Event evt) { }

    protected virtual void OnUpdate() { }

    public void ShowWindow()
    {
        // The event is called before the window opens, so the listeners can consume
        // the event before the window draws the first frame.
        Raylib.ClearWindowState(ConfigFlags.HiddenWindow);

        // Disable the wait for events, because imgui needed to be updated every frame.
        Raylib.DisableEventWaiting();

        // Reset the window position, because the window keeps moving below the PopOS top bar.
        Raylib.SetWindowPosition(0, 0);
    }

    public void HideWindow()
    {
        // When the window is hidden, it's fine to wait for events.
        Raylib.SetWindowState(ConfigFlags.HiddenWindow);
        Raylib.EnableEventWaiting();
    }

    public void CloseWindow()
    {
        Raylib.CloseWindow();
    }

    private void OnEventInternal(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowFocusEvent>(OnWindowFoucsEvent);

        OnEvent(evt);

        layerManager.OnEvent(evt);
    }

    private void OnWindowFoucsEvent(WindowFocusEvent evt)
    {
        // This makes sure the window is the topmost window (i.e: over pip and overlays windows)
        // This also makes it harder to debug the app, so disable it when in debugging.
        // #if DEBUG
        //         if (Debugger.IsAttached)
        //             return;
        // #endif

        if (evt.Focused)
            Raylib.SetWindowState(ConfigFlags.TopmostWindow);
        else
            Raylib.ClearWindowState(ConfigFlags.TopmostWindow);
    }
}

#pragma warning restore CA1822      // CA1822: Mark members as static
#pragma warning restore IDE0130     // IDE0130: Namespace does not match folder structure
