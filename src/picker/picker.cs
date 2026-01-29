using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public class Picker : Application
{
    private readonly ScreenshotProviderLayer screenshotProvider;

    private readonly ColorInfoUILayer colorLayer;
    private readonly PickerOptions options = new();

    private Cursor.State lastCursorState;

    public Picker()
        : base()
    {
        screenshotProvider = new();
        colorLayer = new();

        BackgroundLayer backgroundLayer = new(screenshotProvider);
        MagnifierLayer magnifierLayer = new(screenshotProvider, options);

        layerManager.PushLayer(new InputHandlerLayer(this));
        layerManager.PushLayer(screenshotProvider);
        layerManager.PushLayer(backgroundLayer);
        layerManager.PushLayer(magnifierLayer);

        colorLayer.OnQuit += OnQuitColorLayer;
        colorLayer.OnClose += OnCloseColorLayer;
        magnifierLayer.PickedColor += OnOpenColorLayer;

        EventSystem.OnEvent += OnEvent;

        _ = Cursor.Lock();
    }

    private void OnEvent(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
    }

    private void OnOpenColorLayer(Color color, Vector2 pickPosition)
    {
        colorLayer.Color = color;
        colorLayer.Position = pickPosition;

        layerManager.PushOverlay(colorLayer);

        UnlockCursor(pickPosition);
    }

    private void OnCloseColorLayer()
    {
        layerManager.PopOverlay(colorLayer);
        RestoreCursor();
    }

    private void OnQuitColorLayer()
    {
        OnCloseColorLayer();
        HideWindow();
    }

    private void UnlockCursor(Vector2? position = null)
    {
        lastCursorState = Cursor.Unlock(position);
    }

    private void RestoreCursor()
    {
        Cursor.RestoreState(lastCursorState);
    }

    private void OnWindowClose(WindowCloseEvent evt)
    {
        layerManager.PopOverlay(colorLayer);
    }
}
