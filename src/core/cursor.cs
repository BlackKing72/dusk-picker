using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public static class Cursor
{
    public record struct State(bool WasVisible, bool WasEnabled, Vector2? Position = null);

    private static readonly Observable<bool> isVisisble = new(true);
    private static readonly Observable<bool> isEnabled = new(true);

    public static bool IsVisible
    {
        get => isVisisble.Value;
        set => isVisisble.Value = value;
    }
    public static bool IsEnabled
    {
        get => isEnabled.Value;
        set => isEnabled.Value = value;
    }

    public static Vector2 Position
    {
        get => Raylib.GetMousePosition();
        set => Raylib.SetMousePosition((int)value.X, (int)value.Y);
    }

    static Cursor()
    {
        isVisisble.Changed += (visible) =>
        {
            Action setVisible = visible ? Raylib.ShowCursor : Raylib.HideCursor;
            setVisible();
        };

        isEnabled.Changed += (enabled) =>
        {
            Action setLocked = enabled ? Raylib.EnableCursor : Raylib.DisableCursor;
            setLocked();
        };
    }

    public static State Lock(Vector2? position = null)
    {
        State state = new(IsVisible, IsEnabled, position);

        IsVisible = false;

        if (position.HasValue)
            Position = position.Value;

        IsEnabled = false;

        return state;
    }

    public static State Unlock(Vector2? position = null)
    {
        State state = new(IsVisible, IsEnabled, position);

        IsEnabled = true;

        if (position.HasValue)
            Position = position.Value;

        IsVisible = true;

        return state;
    }

    public static void RestoreState(State state)
    {
        IsVisible = state.WasVisible;

        if (state.Position.HasValue)
            Position = state.Position.Value;

        IsEnabled = state.WasEnabled;
    }
}
