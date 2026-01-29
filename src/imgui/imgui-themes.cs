using System.Numerics;
using Hexa.NET.ImGui;
using IconFonts;
using Raylib_cs;

namespace Black.DuskPicker;

public class ImGuiThemes
{
    public static readonly uint[] FaGlyphRanges = [FontAwesome7.IconMin, FontAwesome7.IconMax, 0];
    public static ImFontPtr FontFaSolid { get; private set; }
    public static ImFontPtr FontMonoRegular { get; private set; }
    public static ImFontPtr FontRegular { get; private set; }
    public static ImFontPtr FontBold { get; private set; }
    public static ImFontPtr FontHeader { get; private set; }

    public static void LoadFonts()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        FontRegular = LoadFont(io, "assets/fonts/quicksand-medium.ttf", 18);
        FontBold = LoadFont(io, "assets/fonts/quicksand-bold.ttf", 18);
        FontMonoRegular = LoadFont(io, "assets/fonts/atkinson_mono-regular.ttf", 18);
        FontHeader = LoadFont(io, "assets/fonts/quicksand-bold.ttf", 24);
        FontFaSolid = LoadFont(io, "assets/fonts/fa-solid.otf", 14, FaGlyphRanges);
    }

    private static unsafe ImFontPtr LoadFont(
        ImGuiIOPtr io,
        string relativePath,
        int size,
        uint[]? ranges = null
    )
    {
        string fontPath = Embedded.ToEmbeddedPath(relativePath);
        Raylib.TraceLog(TraceLogLevel.Info, $"IMGUI: Loading font at: '{fontPath}'");

        byte[] bytes = Embedded.ReadBytes(relativePath);
        uint* glyphRanges = ranges is not null ? ranges.AsPtr() : (uint*)0;

        ImFontPtr font = io.Fonts.AddFontFromMemoryTTF(
            bytes.AsPtr(),
            bytes.Length,
            size,
            glyphRanges
        );

        if (font.IsNull)
            Raylib.TraceLog(TraceLogLevel.Warning, $"IMGUI: Failed to load font at: '{fontPath}'");

        return font;
    }

    /// <summary>
    /// Comfortable Light Orange styleSouthCraftX from ImThemes <br/>
    /// <see href="https://github.com/Patitotective/ImThemes"/>
    /// </summary>
    public static void ApplyConfyLightOrangeTheme()
    {
        var style = ImGui.GetStyle();

        style.Alpha = 1.0000000f;
        style.DisabledAlpha = 1.0000000f;
        style.WindowPadding = new Vector2(20.0f, 20.0f);
        style.WindowRounding = 11.5f;
        style.WindowBorderSize = 0.0f;
        style.WindowMinSize = new Vector2(20.0f, 20.0f);
        style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.None;
        style.ChildRounding = 20.0f;
        style.ChildBorderSize = 1.0000000f;
        style.PopupRounding = 17.4f;
        style.PopupBorderSize = 1.0000000f;
        style.FramePadding = new Vector2(20.0f, 3.4f);
        style.FrameRounding = 11.9f;
        style.FrameBorderSize = 0.0f;
        style.ItemSpacing = new Vector2(8.9f, 13.4f);
        style.ItemInnerSpacing = new Vector2(7.1f, 1.8f);
        style.CellPadding = new Vector2(12.1f, 9.2f);
        style.IndentSpacing = 0.0f;
        style.ColumnsMinSpacing = 8.7f;
        style.ScrollbarSize = 11.6f;
        style.ScrollbarRounding = 15.9f;
        style.GrabMinSize = 3.7f;
        style.GrabRounding = 20.0f;
        style.TabRounding = 9.8f;
        style.TabBorderSize = 0.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

        style.TextColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.TextDisabledColor = new Vector4(0.7254902f, 0.68235296f, 0.54901963f, 1.0f);
        style.WindowBgColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.ChildBgColor = new Vector4(0.90588236f, 0.8980392f, 0.88235295f, 1.0f);
        style.PopupBgColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.BorderColor = new Vector4(0.84313726f, 0.83137256f, 0.80784315f, 1.0f);
        style.BorderShadowColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.FrameBgColor = new Vector4(0.8862745f, 0.8745098f, 0.84705883f, 1.0f);
        style.FrameBgHoveredColor = new Vector4(0.84313726f, 0.83137256f, 0.80784315f, 1.0f);
        style.FrameBgActiveColor = new Vector4(0.84313726f, 0.83137256f, 0.80784315f, 1.0f);
        style.TitleBgColor = new Vector4(0.9529412f, 0.94509804f, 0.92941177f, 1.0f);
        style.TitleBgActiveColor = new Vector4(0.9529412f, 0.94509804f, 0.92941177f, 1.0f);
        style.TitleBgCollapsedColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.MenuBarBgColor = new Vector4(0.9019608f, 0.89411765f, 0.8784314f, 1.0f);
        style.ScrollbarBgColor = new Vector4(0.9529412f, 0.94509804f, 0.92941177f, 1.0f);
        style.ScrollbarGrabColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.ScrollbarGrabHoveredColor = new Vector4(0.84313726f, 0.83137256f, 0.80784315f, 1.0f);
        style.ScrollbarGrabActiveColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.CheckMarkColor = new Vector4(0.96862745f, 0.050980393f, 0.15686275f, 1.0f);
        style.SliderGrabColor = new Vector4(0.9647059f, 0.8f, 0.02745098f, 1.0f);
        style.SliderGrabActiveColor = new Vector4(0.96862745f, 0.5882353f, 0.03529412f, 1.0f);
        style.ButtonColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.ButtonHoveredColor = new Vector4(0.81960785f, 0.8117647f, 0.8039216f, 1.0f);
        style.ButtonActiveColor = new Vector4(0.84705883f, 0.84705883f, 0.84705883f, 1.0f);
        style.HeaderColor = new Vector4(0.85882354f, 0.8352941f, 0.7921569f, 1.0f);
        style.HeaderHoveredColor = new Vector4(0.89411765f, 0.89411765f, 0.89411765f, 1.0f);
        style.HeaderActiveColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.SeparatorColor = new Vector4(0.87058824f, 0.8509804f, 0.80784315f, 1.0f);
        style.SeparatorHoveredColor = new Vector4(0.84313726f, 0.8156863f, 0.7490196f, 1.0f);
        style.SeparatorActiveColor = new Vector4(0.84313726f, 0.8156863f, 0.7490196f, 1.0f);
        style.ResizeGripColor = new Vector4(0.85490197f, 0.85490197f, 0.85490197f, 1.0f);
        style.ResizeGripHoveredColor = new Vector4(0.96862745f, 0.050980393f, 0.15686275f, 1.0f);
        style.ResizeGripActiveColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.TabColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.TabHoveredColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.TabActiveColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.TabUnfocusedColor = new Vector4(0.92156863f, 0.9137255f, 0.8980392f, 1.0f);
        style.TabUnfocusedActiveColor = new Vector4(0.8745098f, 0.7254902f, 0.42745098f, 1.0f);
        style.PlotLinesColor = new Vector4(0.47843137f, 0.4f, 0.29803923f, 1.0f);
        style.PlotLinesHoveredColor = new Vector4(0.9607843f, 0.019607844f, 0.11764706f, 1.0f);
        style.PlotHistogramColor = new Vector4(0.90588236f, 0.6627451f, 0.30980393f, 1.0f);
        style.PlotHistogramHoveredColor = new Vector4(0.6392157f, 0.39607844f, 0.043137256f, 1.0f);
        style.TableHeaderBgColor = new Vector4(0.9529412f, 0.94509804f, 0.92941177f, 1.0f);
        style.TableBorderStrongColor = new Vector4(0.9529412f, 0.94509804f, 0.92941177f, 1.0f);
        style.TableBorderLightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.TableRowBgColor = new Vector4(0.88235295f, 0.8666667f, 0.8509804f, 1.0f);
        style.TableRowBgAltColor = new Vector4(0.9019608f, 0.89411765f, 0.8784314f, 1.0f);
        style.TextSelectedBgColor = new Vector4(0.0627451f, 0.0627451f, 0.0627451f, 1.0f);
        style.DragDropTargetColor = new Vector4(0.5019608f, 0.4862745f, 0.0f, 1.0f);
        style.NavHighlightColor = new Vector4(0.73333335f, 0.70980394f, 0.0f, 1.0f);
        style.NavWindowingHighlightColor = new Vector4(0.5019608f, 0.4862745f, 0.0f, 1.0f);
        style.NavWindowingDimBgColor = new Vector4(0.8039216f, 0.8235294f, 0.45490196f, 0.502f);
        style.ModalWindowDimBgColor = new Vector4(0.8039216f, 0.8235294f, 0.45490196f, 0.502f);
    }

    /// <summary>
    /// Comfortable Dark Cyan styleSouthCraftX from ImThemes <br/>
    /// <see href="https://github.com/Patitotective/ImThemes"/>
    /// </summary>
    public static void ApplyConfyDarkCyanTheme()
    {
        var style = ImGui.GetStyle();

        style.Alpha = 1.0000000f;
        style.DisabledAlpha = 1.0000000f;
        style.WindowPadding = new Vector2(20.0f, 20.0f);
        style.WindowRounding = 11.5f;
        style.WindowBorderSize = 0.0f;
        style.WindowMinSize = new Vector2(20.0f, 20.0f);
        style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.None;
        style.ChildRounding = 20.0f;
        style.ChildBorderSize = 1.0000000f;
        style.PopupRounding = 17.4f;
        style.PopupBorderSize = 1.0000000f;
        style.FramePadding = new Vector2(20.0f, 3.4f);
        style.FrameRounding = 11.9f;
        style.FrameBorderSize = 0.0f;
        style.ItemSpacing = new Vector2(8.9f, 13.4f);
        style.ItemInnerSpacing = new Vector2(7.1f, 1.8f);
        style.CellPadding = new Vector2(12.1f, 9.2f);
        style.IndentSpacing = 0.0f;
        style.ColumnsMinSpacing = 8.7f;
        style.ScrollbarSize = 11.6f;
        style.ScrollbarRounding = 15.9f;
        style.GrabMinSize = 3.7f;
        style.GrabRounding = 20.0f;
        style.TabRounding = 9.8f;
        style.TabBorderSize = 0.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

        style.TextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.TextDisabledColor = new Vector4(0.27450982f, 0.31764707f, 0.4509804f, 1.0f);
        style.WindowBgColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.ChildBgColor = new Vector4(0.09411765f, 0.101960786f, 0.11764706f, 1.0f);
        style.PopupBgColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.BorderColor = new Vector4(0.15686275f, 0.16862746f, 0.19215687f, 1.0f);
        style.BorderShadowColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.FrameBgColor = new Vector4(0.11372549f, 0.1254902f, 0.15294118f, 1.0f);
        style.FrameBgHoveredColor = new Vector4(0.15686275f, 0.16862746f, 0.19215687f, 1.0f);
        style.FrameBgActiveColor = new Vector4(0.15686275f, 0.16862746f, 0.19215687f, 1.0f);
        style.TitleBgColor = new Vector4(0.047058824f, 0.05490196f, 0.07058824f, 1.0f);
        style.TitleBgActiveColor = new Vector4(0.047058824f, 0.05490196f, 0.07058824f, 1.0f);
        style.TitleBgCollapsedColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.MenuBarBgColor = new Vector4(0.09803922f, 0.105882354f, 0.12156863f, 1.0f);
        style.ScrollbarBgColor = new Vector4(0.047058824f, 0.05490196f, 0.07058824f, 1.0f);
        style.ScrollbarGrabColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.ScrollbarGrabHoveredColor = new Vector4(0.15686275f, 0.16862746f, 0.19215687f, 1.0f);
        style.ScrollbarGrabActiveColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.CheckMarkColor = new Vector4(0.03137255f, 0.9490196f, 0.84313726f, 1.0f);
        style.SliderGrabColor = new Vector4(0.03137255f, 0.9490196f, 0.84313726f, 1.0f);
        style.SliderGrabActiveColor = new Vector4(0.6f, 0.9647059f, 0.03137255f, 1.0f);
        style.ButtonColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.ButtonHoveredColor = new Vector4(0.18039216f, 0.1882353f, 0.19607843f, 1.0f);
        style.ButtonActiveColor = new Vector4(0.15294118f, 0.15294118f, 0.15294118f, 1.0f);
        style.HeaderColor = new Vector4(0.14117648f, 0.16470589f, 0.20784314f, 1.0f);
        style.HeaderHoveredColor = new Vector4(0.105882354f, 0.105882354f, 0.105882354f, 1.0f);
        style.HeaderActiveColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.SeparatorColor = new Vector4(0.12941177f, 0.14901961f, 0.19215687f, 1.0f);
        style.SeparatorHoveredColor = new Vector4(0.15686275f, 0.18431373f, 0.2509804f, 1.0f);
        style.SeparatorActiveColor = new Vector4(0.15686275f, 0.18431373f, 0.2509804f, 1.0f);
        style.ResizeGripColor = new Vector4(0.14509805f, 0.14509805f, 0.14509805f, 1.0f);
        style.ResizeGripHoveredColor = new Vector4(0.03137255f, 0.9490196f, 0.84313726f, 1.0f);
        style.ResizeGripActiveColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.TabColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.TabHoveredColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.TabActiveColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.TabUnfocusedColor = new Vector4(0.078431375f, 0.08627451f, 0.101960786f, 1.0f);
        style.TabUnfocusedActiveColor = new Vector4(0.1254902f, 0.27450982f, 0.57254905f, 1.0f);
        style.PlotLinesColor = new Vector4(0.52156866f, 0.6f, 0.7019608f, 1.0f);
        style.PlotLinesHoveredColor = new Vector4(0.039215688f, 0.98039216f, 0.98039216f, 1.0f);
        style.PlotHistogramColor = new Vector4(0.03137255f, 0.9490196f, 0.84313726f, 1.0f);
        style.PlotHistogramHoveredColor = new Vector4(0.15686275f, 0.18431373f, 0.2509804f, 1.0f);
        style.TableHeaderBgColor = new Vector4(0.047058824f, 0.05490196f, 0.07058824f, 1.0f);
        style.TableBorderStrongColor = new Vector4(0.047058824f, 0.05490196f, 0.07058824f, 1.0f);
        style.TableBorderLightColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.TableRowBgColor = new Vector4(0.11764706f, 0.13333334f, 0.14901961f, 1.0f);
        style.TableRowBgAltColor = new Vector4(0.09803922f, 0.105882354f, 0.12156863f, 1.0f);
        style.TextSelectedBgColor = new Vector4(0.9372549f, 0.9372549f, 0.9372549f, 1.0f);
        style.DragDropTargetColor = new Vector4(0.49803922f, 0.5137255f, 1.0f, 1.0f);
        style.NavHighlightColor = new Vector4(0.26666668f, 0.2901961f, 1.0f, 1.0f);
        style.NavWindowingHighlightColor = new Vector4(0.49803922f, 0.5137255f, 1.0f, 1.0f);
        style.NavWindowingDimBgColor = new Vector4(0.19607843f, 0.1764706f, 0.54509807f, 0.5f);
        style.ModalWindowDimBgColor = new Vector4(0.19607843f, 0.1764706f, 0.54509807f, 0.5f);
    }
}
