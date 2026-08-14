using System.Numerics;
using Hexa.NET.ImGui;
using Raylib_cs;

namespace Black.DuskPicker;

public class ColorPreviewLayer : Layer
{
    private readonly StateProvider stateProvider;
    private readonly ScreenshotProvider screenshotProvider;

    private readonly ImGuiPopup previewPopup;

    public ColorPreviewLayer(StateProvider stateProvider, ScreenshotProvider screenshotProvider)
    {
        this.stateProvider = stateProvider;
        this.screenshotProvider = screenshotProvider;

        previewPopup = new("color-preview")
        {
            MinWidth = 300,
            MaxWidth = 300,

            Flags = ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoSavedSettings,
        };
    }

    public override void OnAttach()
    {
        previewPopup.Open(stateProvider.MouseTexPosition);
    }

    public override void OnDetach()
    {
        previewPopup.Close();
    }

    public override void OnGui()
    {
        Color previewColor = screenshotProvider.GetColorAt(stateProvider.MouseTexPosition);
        ColorPacked similarColor = stateProvider.GetSimilarColor(previewColor);

        Vector4 color = new(
            previewColor.R / 255.0f,
            previewColor.G / 255.0f,
            previewColor.B / 255.0f,
            previewColor.A / 255.0f
        );

        ImGuiStylePtr style = ImGui.GetStyle();

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.WindowPadding * 0.25f);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, style.CellPadding * new Vector2(1.0f, 0.25f));

        previewPopup.Position = stateProvider.MousePosition;
        previewPopup.OnGui(() =>
        {
            DrawPreviewColor(color);
            ImGui.SameLine();
            DrawPreviewInfo(previewColor, similarColor);
        });

        ImGui.PopStyleVar(2);
    }

    private static unsafe void DrawPreviewColor(Vector4 color)
    {
        ImGui.SetNextWindowSizeConstraints(
            Vector2.Zero,
            Vector2.One * 72.0f,
            ImSizeConstrains.Square
        );

        if (ImGui.BeginChild("quick-preview-panel"))
        {
            Vector2 availSize = ImGui.GetContentRegionAvail();
            ImGui.ColorButton("color-preview-panel-color", color, availSize);
        }

        ImGui.EndChild();
    }

    private static readonly RGB255ColorFormatter rgbFormatter = new();
    private static readonly HexColorFormatter hexFormatter = new();

    private static void DrawPreviewInfo(Color color, ColorPacked similarColor)
    {
        ImGuiTableFlags flags = ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.NoBordersInBody;

        if (ImGui.BeginTable("color-preview-panel-info", 1, flags))
        {
            TableNextRow();

            ImGui.PushFont(ImGuiThemes.FontBold);
            ImGui.Text($"{similarColor.Name}");
            ImGui.PopFont();

            ImGui.PushFont(ImGuiThemes.FontMonoRegular);

            TableNextRow();
            ImGui.Text($"Rgb: {rgbFormatter.ToDisplayString(color)}");

            TableNextRow();
            ImGui.Text($"Hex: {hexFormatter.ToDisplayString(color)}");

            ImGui.PopFont();

            ImGui.EndTable();
        }

        static void TableNextRow()
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
        }
    }
}
