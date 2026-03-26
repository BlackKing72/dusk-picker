using Hexa.NET.ImGui;
using IconFonts;
using Raylib_cs;

namespace Black.DuskPicker;

public class ColorInfoLayer : Layer
{
    private readonly StateProvider stateProvider;
    private readonly ImGuiPopup popup;

    private ColorPacked similarColor = ColorPacked.White;
    private Color color = Color.White;

    public ColorInfoLayer(StateProvider stateProvider)
    {
        this.stateProvider = stateProvider;

        popup = new ImGuiPopup("color-info") { MinWidth = 300.0f, MaxWidth = 300.0f };
        popup.OnClosePopup += wasClosedByUser =>
        {
            // only do something if the user closed the popup, the
            // other cases are already handled.
            if (wasClosedByUser)
                stateProvider.DismissColor();
        };
    }

    public override void OnAttach()
    {
        (color, similarColor) = stateProvider.PickColorInfo;
        popup.Open(stateProvider.PickTexPosition);
    }

    public override void OnDetach()
    {
        popup.Close();
    }

    // block the update on other layers.
    public override bool OnUpdate() => true;

    public override void OnGui()
    {
        popup.OnGui(() =>
        {
            ImGui.PushFont(ImGuiThemes.FontRegular);

            DrawHeaderButtons();
            DrawHeader();
            DrawContent();

            ImGui.PopFont();
        });
    }

    private void DrawHeader()
    {
        ImGui.PushFont(ImGuiThemes.FontHeader);

        ImGui.Text($"{similarColor.Name}");
        ImGui.Separator();

        ImGui.PopFont();
    }

    private void RequestDismiss()
    {
        popup.Close();
        stateProvider.DismissColor();
    }

    private void RequestQuit()
    {
        popup.Close();
        stateProvider.RequestQuit();
    }

    private void DrawContent()
    {
        foreach (ColorFormatter parser in ColorFormatter.Formatters)
        {
            string info = parser.ToDisplayString(color);
            if (ImGuiWidgets.CopiableText(parser.Label, info))
            {
                Raylib.SetClipboardText(info);
                RequestQuit();
            }
        }
    }

    private void DrawHeaderButtons()
    {
        float frameSize = ImGui.GetFrameHeight();

        ImGuiWidgets.HLayout(
            new(frameSize),
            ImGuiWidgets.Parent.Window,
            ImGuiWidgets.Aligment.Right,
            [drawPickButton, drawCloseButton]
        );

        void drawPickButton()
        {
            string icon = FontAwesome7.EyeDropper;
            if (ImGuiWidgets.IconButton("request-dismiss", icon, "Pick another color"))
                RequestDismiss();
        }

        void drawCloseButton()
        {
            string icon = FontAwesome7.Xmark;
            if (ImGuiWidgets.IconButton("request-quit", icon, "Close"))
                RequestQuit();
        }
    }
}
