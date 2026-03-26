using System.Numerics;
using Hexa.NET.ImGui;

namespace Black.DuskPicker;

public class ImGuiPopup(string id)
{
    /// <remarks>
    /// If `true` the user have closed the popup manually by pressing escape or clicking outside,
    /// otherwise the popup was closed by calling the close method.
    /// </remarks>
    public event Action<bool>? OnClosePopup;

    public float Margin { get; set; } = 16.0f;
    public float MinWidth { get; set; } = 0;
    public float MinHeight { get; set; } = 0;
    public float MaxWidth { get; set; } = float.MaxValue;
    public float MaxHeight { get; set; } = float.MaxValue;
    public ImGuiWindowFlags Flags { get; set; } = ImGuiWindowFlags.None;
    public Vector2 Position { get; set; } = Vector2.Zero;

    private readonly string popupId = $"popup__{id}";

    private bool isOpenRequested = false;
    private bool wasOpenLastFrame = false;
    private Vector2 lastWindowSize = -Vector2.One;

    public void OnGui(Action renderContentFn)
    {
        if (isOpenRequested)
        {
            isOpenRequested = false;
            ImGui.OpenPopup(popupId);
        }

        SetPositionAndAnchors();

        if (ImGui.BeginPopup(popupId, Flags | ImGuiWindowFlags.NoSavedSettings))
        {
            lastWindowSize = ImGui.GetWindowSize();
            wasOpenLastFrame = true;

            renderContentFn();

            ImGui.EndPopup();
        }
        else if (wasOpenLastFrame)
        {
            // the popup was closed by the user, either by clicking
            // outside or pressing the escape key
            wasOpenLastFrame = false;
            OnClosePopup?.Invoke(true);
        }
    }

    public void Open(Vector2 position)
    {
        isOpenRequested = true;
        this.Position = position;
    }

    public void Close()
    {
        if (wasOpenLastFrame)
            ImGui.CloseCurrentPopup();

        OnClosePopup?.Invoke(false);
    }

    private void SetPositionAndAnchors()
    {
        Vector2 margin = new(Margin);

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        Vector2 workSize = viewport.WorkSize;

        // a good default for the first frame. this makes sure the popup is
        // never clipping the cursor or outside the viewport.
        if (lastWindowSize == -Vector2.One)
            lastWindowSize = workSize * 0.5f;

        // account for the margin in both the start and end position.
        Vector2 min = Position + margin;
        Vector2 max = min + lastWindowSize + margin;

        // flip means swap the location where the anchors is set.
        // flip x swaps left-right and vice versa, flip y it's for top-bottom
        bool flipAnchorX = max.X >= workSize.X;
        bool flipAnchorY = max.Y >= workSize.Y;

        // calculate the offset and anchor based on where the popup is in
        // relation with the viewport. the default is top-left with a positive offset.
        Vector2 offset = new(x: flipAnchorX ? -Margin : Margin, y: flipAnchorY ? -Margin : Margin);
        Vector2 anchor = new(x: flipAnchorX ? 1.0f : 0.0f, y: flipAnchorY ? 1.0f : 0.0f);

        ImGui.SetNextWindowPos(Position + offset, ImGuiCond.Always, anchor);
        ImGui.SetNextWindowSizeConstraints(new(MinWidth, MinHeight), new(MaxWidth, MaxHeight));
    }
}
