using System.Numerics;
using Hexa.NET.ImGui;
using IconFonts;

namespace Black.DuskPicker;

public static class ImGuiWidgets
{
    public static bool IconButton(string id, string icon, string tooltip = "")
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float frameSize = ImGui.GetFrameHeight();

        ImGui.PushFont(ImGuiThemes.FontFaSolid);

        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, style.ButtonHoveredColor with { W = 0.05f });
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, style.ButtonActiveColor with { W = 0.125f });
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(style.FramePadding.Y * 0.5f));

        bool isOpen = ImGui.Button($"{icon}###{id}", new(frameSize));

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar(2);
        ImGui.PopFont();

        bool hasTooltip = !string.IsNullOrWhiteSpace(tooltip);
        bool isHovered = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
        if (hasTooltip && isHovered)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.WindowPadding * 0.75f);
            ImGui.SetTooltip(tooltip);
            ImGui.PopStyleVar();
        }

        return isOpen;
    }

    public static bool CopiableText(string label, string info)
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, style.ItemSpacing * 0);

        // draw label with default bold font.
        ImGui.PushFont(ImGuiThemes.FontBold);
        // ImGui.AlignTextToFramePadding();
        ImGui.Text($"{label}:");
        ImGui.PopFont();

        // ImGui.SameLine();

        ImGui.BeginGroup();

        var availRegion = ImGui.GetContentRegionAvail();

        // draw info with mono regular font.
        ImGui.PushFont(ImGuiThemes.FontMonoRegular);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(info);
        ImGui.PopFont();

        ImGui.SameLine(availRegion.X - ImGui.GetFrameHeightWithSpacing());
        bool isOpen = IconButton($"{label}__copy", FontAwesome7.Copy, "Copy");

        ImGui.PopStyleVar();

        ImGui.EndGroup();

        return isOpen;
    }

    public enum Aligment
    {
        Left,
        Center,
        Right,
    }

    public enum Parent
    {
        Window,
        Cursor,
    }

    public record struct HLayoutOptions(
        Aligment Aligment = Aligment.Center,
        Vector2? ItemSpacing = null
    );

    public static void HLayout(
        Vector2 itemSize,
        Parent parent = Parent.Cursor,
        Aligment aligment = Aligment.Center,
        params ReadOnlySpan<Action> actions
    )
    {
        var style = ImGui.GetStyle();

        Vector2 regionStart =
            parent is Parent.Window ? ImGui.GetWindowPos() : ImGui.GetCursorScreenPos();
        Vector2 regionSize =
            parent is Parent.Window ? ImGui.GetWindowSize() : ImGui.GetContentRegionAvail();

        Vector2 itemSpacing = style.ItemSpacing;
        Vector2 maxRegionSize = regionStart + regionSize;

        float maxItemSize = (itemSize.X * actions.Length) + (itemSpacing.X * (actions.Length - 1));

        float y = regionStart.Y + style.WindowPadding.Y;
        float x = aligment switch
        {
            Aligment.Left => regionStart.X + style.WindowPadding.X,
            Aligment.Right => maxRegionSize.X - maxItemSize - style.WindowPadding.X,
            _ or Aligment.Center => regionStart.X + ((regionSize.X * 0.5f) - (maxItemSize * 0.5f)),
        };

        ImGui.SetCursorScreenPos(new(x, y));
        float offset = maxItemSize / actions.Length;

        for (int i = 0; i < actions.Length; i++)
        {
            if (i > 0)
                ImGui.SameLine();

            actions[i]();
        }
    }

    public static void Icon(string icon)
    {
        ImGui.PushFont(ImGuiThemes.FontFaSolid);
        ImGui.Text(icon);
        ImGui.PopFont();
    }

    public static void IconText(string icon, string text)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemSpacing * 2);

        Icon(icon);
        ImGui.SameLine();
        ImGui.Text(text);

        ImGui.PopStyleVar();
    }
}
