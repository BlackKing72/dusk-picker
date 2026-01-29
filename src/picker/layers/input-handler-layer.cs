using Raylib_cs;

namespace Black.DuskPicker;

public class InputHandlerLayer(Picker picker) : Layer
{
    public override bool OnUpdate()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            picker.HideWindow();
        }

        return false;
    }
}
