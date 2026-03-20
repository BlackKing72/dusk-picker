namespace Black.DuskPicker;

public enum PickerPalette
{
    Web, Toyz, Xkcd,
}

public static class PickerPaletteX
{
    extension(PickerPalette palette)
    {
        public ColorDatabase AsDatabase => palette switch
        {
            PickerPalette.Web => ColorDatabase.Web,
            PickerPalette.Toyz => ColorDatabase.Toyz,
            PickerPalette.Xkcd => ColorDatabase.Xkcd,
            _ => throw new ArgumentOutOfRangeException(nameof(palette), $"Palette value was invalid. Value: ${(int)palette}."),
        };
    }
}

public struct PickerOptions()
{
    public float ZoomSensitivity { get; init; } = 0.5f;
    public float RadiusSensitivity { get; init; } = 1.0f;
    public float MoveSensitivityAtMinZoom { get; init; } = 1.0f;
    public float MoveSensitivityAtMaxZoom { get; init; } = 0.05f;

    public float MinMagnifierZoom { get; init; } = 1.0f;
    public float MaxMagnifierZoom { get; init; } = 20.0f;
    public float MinMagnifierRadius { get; init; } = 10.0f;
    public float MaxMagnifierRadius { get; init; } = float.MaxValue;

    public PickerPalette Palette { get; init; } = PickerPalette.Xkcd;
}
