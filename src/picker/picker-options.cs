namespace Black.DuskPicker;

public struct PickerOptions()
{
    public float ZoomSensitivity { get; init; } = 100.0f;
    public float RadiusSensitivity { get; init; } = 1000.0f;
    public float MoveSensitivityAtMinZoom { get; init; } = 1.0f;
    public float MoveSensitivityAtMaxZoom { get; init; } = 0.05f;

    public float MinMagnifierZoom { get; init; } = 1.0f;
    public float MaxMagnifierZoom { get; init; } = 20.0f;
    public float MinMagnifierRadius { get; init; } = 10.0f;
    public float MaxMagnifierRadius { get; init; } = float.MaxValue;
}
