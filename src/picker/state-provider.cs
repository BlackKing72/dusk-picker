using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public class StateProvider(PickerPalette colorPalette = PickerPalette.Xkcd)
{
    public event Action? OnColorSelected;
    public event Action? OnColorDismissed;
    public event Action? OnQuitRequested;

    public Color PickColor { get; private set; } = Color.RayWhite;
    public ColorPacked SimilarColor { get; private set; } = ColorPacked.White;
    public Vector2 PickPosition { get; private set; } = Vector2.Zero;

    private readonly ColorDatabase colorDatabase = colorPalette.AsDatabase;
    private Cursor.State lastCursorState;

    public void SelectColor(Color color, Vector2 position)
    {
        PickColor = color;
        PickPosition = position;

        SimilarColor = ColorUtils.GetPerceptualSimilarColor(
            colorDatabase,
            color.R,
            color.G,
            color.B
        );

        OnColorSelected?.Invoke();

        UnlockCursor(position);
    }

    public void DismissColor()
    {
        OnColorDismissed?.Invoke();
        RestoreCursor();
    }

    public void RequestQuit()
    {
        DismissColor();
        OnQuitRequested?.Invoke();
    }

    public void OnEvent(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
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
        OnColorDismissed?.Invoke();
    }
}
