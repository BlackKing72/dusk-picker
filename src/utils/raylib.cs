using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public static class RaylibX
{
    extension(Raylib)
    {
        public static void SetWindowPosition(Vector2 position) => Raylib.SetWindowPosition((int)position.X, (int)position.Y);
        public static void SetWindowSize(Vector2 size) => Raylib.SetWindowSize((int)size.X, (int)size.Y);

        // info: this extensions are generating a false warning (CS8620 - "Argument cannot be used for parameter due to
        // differences in the nullability of reference types"). apparently this is a know issue in roslyn.
        // see: https://github.com/dotnet/runtime/issues/121597 and https://github.com/dotnet/roslyn/issues/80024

        public static bool IsKeyUp(params ReadOnlySpan<KeyboardKey> keys) => IsAnyKey(Raylib.IsKeyUp, keys);
        public static bool IsKeyDown(params ReadOnlySpan<KeyboardKey> keys) => IsAnyKey(Raylib.IsKeyDown, keys);
        public static bool IsKeyPressed(params ReadOnlySpan<KeyboardKey> keys) => IsAnyKey(Raylib.IsKeyPressed, keys);
        public static bool IsKeyReleased(params ReadOnlySpan<KeyboardKey> keys) => IsAnyKey(Raylib.IsKeyReleased, keys);
        public static bool IsKeyPressedRepeat(params ReadOnlySpan<KeyboardKey> keys) => IsAnyKey(Raylib.IsKeyPressedRepeat, keys);

        public static bool IsMouseButtonUp(params ReadOnlySpan<MouseButton> buttons) => IsAnyMouseButton(Raylib.IsMouseButtonUp, buttons);
        public static bool IsMouseButtonDown(params ReadOnlySpan<MouseButton> buttons) => IsAnyMouseButton(Raylib.IsMouseButtonDown, buttons);
        public static bool IsMouseButtonPressed(params ReadOnlySpan<MouseButton> buttons) => IsAnyMouseButton(Raylib.IsMouseButtonPressed, buttons);
        public static bool IsMouseButtonReleased(params ReadOnlySpan<MouseButton> buttons) => IsAnyMouseButton(Raylib.IsMouseButtonReleased, buttons);

        private static CBool IsAnyKey(Func<KeyboardKey, CBool> predicate, params ReadOnlySpan<KeyboardKey> keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (predicate(keys[i]))
                    return true;
            }

            return false;
        }

        private static CBool IsAnyMouseButton(Func<MouseButton, CBool> predicate, params ReadOnlySpan<MouseButton> buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (predicate(buttons[i]))
                    return true;
            }

            return false;
        }
    }
}

public static class RectangleX
{
    extension(Rectangle rect)
    {
        public bool IsInside(Vector2 point)
        {
            return point.X >= rect.X
                && point.Y >= rect.Y
                && point.X <= rect.X + rect.Width
                && point.Y <= rect.Y + rect.Height;
        }
    }
}

public struct ShaderUniform(string name)
{
    private int? location;

    public void SetValue<T>(Shader shader, T value, ShaderUniformDataType uniformType) where T : unmanaged
    {
        location ??= Raylib.GetShaderLocation(shader, name);
        Raylib.SetShaderValue<T>(shader, location.Value, value, uniformType);
    }
}
