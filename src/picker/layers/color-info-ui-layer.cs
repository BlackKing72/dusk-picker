using System.Numerics;
using Hexa.NET.ImGui;
using IconFonts;
using Raylib_cs;

namespace Black.DuskPicker;

public class ColorInfoUILayer(PickerPalette colorPalette = PickerPalette.Xkcd) : Layer
{
    public event Action? OnClose;
    public event Action? OnQuit;

    private readonly Vector2 Offset = Vector2.One * 16;

    public Vector2 Position { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;

    private ColorDatabase colorDatabase = colorPalette.AsDatabase;
    private ColorPacked similarColor = ColorPacked.White;
    private bool wasOpenLastFrame;
    private bool requestedOpen;

    public override void OnAttach()
    {
        similarColor = ColorUtils.GetPerceptualSimilarColor(
            colorDatabase,
            Color.R,
            Color.G,
            Color.B
        );

        requestedOpen = true;
    }

    // block the update on other layers.
    public override bool OnUpdate() => true;

    [Flags]
    private enum AnchorPosition
    {
        Top = 1 << 0,
        Bottom = 1 << 1,
        Right = 1 << 2,
        Left = 1 << 3,
    };

    private bool flipAnchorY = false;
    private bool flipAnchorX = false;

    public override void OnGui()
    {
        if (requestedOpen)
        {
            requestedOpen = false;
            ImGui.OpenPopup(nameof(ColorInfoUILayer));
        }

        var viewport = ImGui.GetMainViewport();
        var workSize = viewport.WorkSize - new Vector2(8.0f);

        Vector2 offset = new(
            x: flipAnchorX ? -Offset.X : Offset.X,
            y: flipAnchorY ? -Offset.Y : Offset.Y
        );

        Vector2 anchor = new(x: flipAnchorX ? 1.0f : 0.0f, y: flipAnchorY ? 1.0f : 0.0f);

        ImGui.SetNextWindowPos(Position + offset, ImGuiCond.Always, anchor);
        ImGui.SetNextWindowSizeConstraints(new(300, 0), new(300, float.MaxValue));

        if (ImGui.BeginPopup(nameof(ColorInfoUILayer), ImGuiWindowFlags.NoSavedSettings))
        {
            var min = Position + Offset;
            var max = min + ImGui.GetWindowSize();

            flipAnchorX = max.X >= workSize.X;
            flipAnchorY = max.Y >= workSize.Y;

            DrawPopup();
            ImGui.EndPopup();
        }
        else if (wasOpenLastFrame)
        {
            // the popup is closed, and it happened this frame.
            wasOpenLastFrame = false;
            ClosePopup();
        }
    }

    private void DrawPopup()
    {
        var style = ImGui.GetStyle();

        wasOpenLastFrame = true;

        ImGui.PushFont(ImGuiThemes.FontRegular);
        ImGui.PushFont(ImGuiThemes.FontHeader);

        // draw title / color name
        ImGui.Text($"{similarColor.Name}");
        ImGui.Separator();
        ImGui.PopFont();

        foreach (var parser in ColorParser.Parsers)
        {
            string info = parser.ToDisplayString(Color);
            if (ImGuiWidgets.CopiableText(parser.Label, info))
            {
                Raylib.SetClipboardText(info);
                ClosePopup(true, true);
            }
        }

        var pos = ImGui.GetWindowPos();
        var size = ImGui.GetWindowSize();
        float frameSize = ImGui.GetFrameHeight();

        ImGuiWidgets.HLayout(
            new(frameSize),
            ImGuiWidgets.Parent.Window,
            ImGuiWidgets.Aligment.Right,
            [DrawPickButton, DrawCloseButton]
        );

        ImGui.PopFont();
    }

    private void DrawPickButton()
    {
        if (ImGuiWidgets.IconButton("__pick", FontAwesome7.EyeDropper, "Pick another color"))
            ClosePopup(true);
    }

    private void DrawCloseButton()
    {
        if (ImGuiWidgets.IconButton("__close", FontAwesome7.Xmark, "Close"))
            ClosePopup(true, true);
    }

    private void ClosePopup(bool isCurrentPopup = false, bool requestQuitCommand = false)
    {
        if (isCurrentPopup)
            ImGui.CloseCurrentPopup();

        Action? callback = requestQuitCommand ? OnQuit : OnClose;
        callback?.Invoke();
    }
}

public abstract class ColorParser
{
    public static ColorParser[] Parsers { get; } =
        [new RGB255ColorParser(), new RGB1ColorParser(), new HexColorParser()];

    public abstract string Label { get; }
    public abstract string ToDisplayString(Color color);
}

public class RGB255ColorParser : ColorParser
{
    public override string Label => "RGB (0-255)";

    public override string ToDisplayString(Color color)
    {
        return $"{color.R,3}, {color.G,3}, {color.B,3}";
    }
}

public class RGB1ColorParser : ColorParser
{
    public override string Label => "RGB (0-1)";

    public override string ToDisplayString(Color color)
    {
        float r = color.R / 255.0f;
        float g = color.G / 255.0f;
        float b = color.B / 255.0f;

        return $"{r:F3}, {g:F3}, {b:F3}";
    }
}

public class HexColorParser(HexColorParser.Options? options = null) : ColorParser
{
    public record struct Options(bool ShowHashtag = true, bool Lowercase = true);

    public override string Label => "Hex";

    public override string ToDisplayString(Color color)
    {
        Options opt = options ?? new();

        string prefix = opt.ShowHashtag ? "#" : "";

        string r = Convert.ToString(color.R, 16);
        string g = Convert.ToString(color.G, 16);
        string b = Convert.ToString(color.B, 16);

        string hex = $"{prefix}{r:X2}{g:X2}{b:X2}";

        if (!opt.Lowercase)
            hex = hex.ToUpperInvariant();

        return hex;
    }
}
