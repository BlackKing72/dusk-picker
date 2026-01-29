using Black.DuskPicker.ImRaylib;
using Hexa.NET.ImGui;

namespace Black.DuskPicker;

public class GuiManager : IDisposable
{
    private readonly ImGuiContextPtr context;

    public GuiManager(bool prefersDarkTheme = true)
    {
        context = ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();

        unsafe
        {
            // This prevents ImGui from creating a 'imgui.ini' file
            io.IniFilename = null;
        }

        // Set this flag so ImGui don't control the cursor lock/visibility.
        io.ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange;

        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        // io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        // io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        Action applyTheme = prefersDarkTheme
            ? ImGuiThemes.ApplyConfyDarkCyanTheme
            : ImGuiThemes.ApplyConfyLightOrangeTheme;

        ImGuiThemes.LoadFonts();
        applyTheme();

        _ = GuiRenderer.Init();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing = false)
    {
        if (!disposing)
        {
            Console.WriteLine($"Gui Renderer was not properly disposed.");
        }

        GuiRenderer.Shutdown();
        ImGui.DestroyContext(context);
    }

    public void BeginGui()
    {
        GuiRenderer.NewFrame();
        ImGui.NewFrame();
        ImGui.PushFont(ImGuiThemes.FontRegular);
    }

    public unsafe void EndGui()
    {
        ImGui.PopFont();
        ImGui.Render();
        GuiRenderer.RenderDrawData(ImGui.GetDrawData());
    }
}
