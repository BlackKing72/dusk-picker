namespace Black.DuskPicker;

public static class ColorUtils
{
    public static ColorPacked GetSimilarColor(ColorDatabase database, byte r, byte g, byte b)
    {
        return database
            .Colors.Index()
            .Select(color =>
            {
                ColorPacked c = color.Item;
                double d = Math.Sqrt(
                    Math.Pow(r - c.R, 2.0) + Math.Pow(g - c.G, 2.0) + Math.Pow(b - c.B, 2.0)
                );
                return (color.Index, color.Item, Difference: d);
            })
            .OrderBy(color => color.Difference)
            .First()
            .Item;
    }

    public static ColorPacked GetPerceptualSimilarColor(
        ColorDatabase database,
        byte r,
        byte g,
        byte b
    )
    {
        return database
            .Colors.Index()
            .Select(color =>
            {
                ColorPacked c = color.Item;
                double d = Math.Sqrt(
                    (0.299 * Math.Pow(r - c.R, 2.0))
                        + (0.587 * Math.Pow(g - c.G, 2.0))
                        + (0.114 * Math.Pow(b - c.B, 2.0))
                );
                return (color.Index, color.Item, Difference: d);
            })
            .OrderBy(color => color.Difference)
            .First()
            .Item;
    }
}
