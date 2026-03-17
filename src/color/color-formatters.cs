using Raylib_cs;

namespace Black.DuskPicker;

public abstract class ColorFormatter
{
    public static ColorFormatter[] Formatters { get; } =
        [new RGB255ColorFormatter(), new RGBColorFormatter(), new HexColorFormatter()];

    public abstract string Label { get; }
    public abstract string ToDisplayString(Color color);
}

public class RGB255ColorFormatter : ColorFormatter
{
    public override string Label => "RGB (0-255)";

    public override string ToDisplayString(Color color)
    {
        return $"{color.R}, {color.G}, {color.B}";
    }
}

public class RGBColorFormatter : ColorFormatter
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

public class HexColorFormatter(HexColorFormatter.Options? options = null) : ColorFormatter
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
