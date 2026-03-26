using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public record struct ColorInfo(Color Color, ColorPacked SimilarColor);

public class StateProvider(PickerPalette colorPalette = PickerPalette.Xkcd)
{
    public event Action? OnColorSelected;
    public event Action? OnColorDismissed;
    public event Action? OnQuitRequested;

    /// <summary> Last selected color and similar color </summary>
    public ColorInfo PickColorInfo { get; private set; }

    /// <summary> Last selected position in window/screen space </summary>
    public Vector2 PickTexPosition { get; private set; } = Vector2.Zero;

    /// <summary> Current mouse position in texture space </summary>
    public Vector2 MouseTexPosition { get; set; } = Vector2.Zero;

    /// <summary> Current mouse position in window/screen space </summary>
    public Vector2 MousePosition { get; set; } = Vector2.Zero;

    private readonly ColorDatabase colorDatabase = colorPalette.AsDatabase;
    private CursorState lastCursorState;

    public void SelectColor(Color color, Vector2 position)
    {
        PickColorInfo = new ColorInfo(color, GetSimilarColor(color));
        PickTexPosition = position;

        OnColorSelected?.Invoke();

        UnlockCursor(position);
    }

    public ColorPacked GetSimilarColor(Color color)
    {
        return ColorUtils.GetPerceptualSimilarColor(colorDatabase, color.R, color.G, color.B);
    }

    public void DismissColor()
    {
        OnColorDismissed?.Invoke();
        RestoreCursor();
    }

    public void RequestQuit()
    {
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
