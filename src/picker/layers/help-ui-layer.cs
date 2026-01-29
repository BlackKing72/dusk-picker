using System.Numerics;
using Hexa.NET.ImGui;
using IconFonts;
using Raylib_cs;

namespace Black.DuskPicker;

public class HelpUILayer : Layer
{
    public Action? OnOpen;
    public Action? OnClose;

    private bool isHelpOpen = false;

    public override bool OnUpdate() => isHelpOpen;

    public override void OnGui()
    {
        if (!isHelpOpen && Raylib.IsKeyPressed(KeyboardKey.F1))
        {
            OpenPopup();
        }

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGuiStylePtr style = ImGui.GetStyle();
        ImGuiWindowFlags flags =
            ImGuiWindowFlags.UnsavedDocument
            | ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.NoResize;

        // ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.WindowPadding * 0.5f);
        // ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, style.ItemSpacing * 0.25f);

        Vector2 center = ImGui.GetWorkCenter(viewport);
        Vector2 size = viewport.WorkSize * new Vector2(0.5f, 0.75f);
        Vector2 position = center - size * 0.5f;

        ImGui.SetNextWindowSize(size);
        ImGui.SetNextWindowPos(position);
        ImGui.SetNextWindowViewport(viewport.ID);

        if (ImGui.BeginPopupModal(nameof(HelpUILayer), ref isHelpOpen, flags))
        {
            ImGui.UseFont(ImGuiThemes.FontHeader, () => ImGui.Text("Help"));
            ImGui.Separator();

            ImGuiWidgets.HLayout(
                new(ImGui.GetFrameHeight()),
                ImGuiWidgets.Parent.Window,
                ImGuiWidgets.Aligment.Right,
                [
                    () =>
                    {
                        if (ImGuiWidgets.IconButton("__close", FontAwesome7.Xmark, "Close"))
                            ClosePopup();
                    },
                ]
            );

            ImGui.EndPopup();
        }

        // ImGui.PopStyleVar(2);
    }

    private void OpenPopup()
    {
        ImGui.OpenPopup(nameof(HelpUILayer));
        isHelpOpen = true;
        OnOpen?.Invoke();
    }

    private void ClosePopup()
    {
        isHelpOpen = false;
        OnClose?.Invoke();
    }
}
