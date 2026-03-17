using Raylib_cs;

namespace Black.DuskPicker;

public class InputHandlerLayer(StateProvider stateProvider) : Layer
{
    public override bool OnUpdate()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            stateProvider.RequestQuit();
        }

        return false;
    }
}
