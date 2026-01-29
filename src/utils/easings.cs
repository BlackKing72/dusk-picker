namespace Black.DuskPicker;

using static System.MathF;

public static class Easings
{
    public static float EaseInQuart(float x)
    {
        return x * x * x * x;
    }

    public static float EaseOutQuart(float x)
    {
        return 1.0f - Pow(1.0f - x, 4);
    }

    public static float EaseInOutQuart(float x)
    {
        return x < 0.5f ? 8.0f * x * x * x * x : 1.0f - (Pow((-2.0f * x) + 2.0f, 4.0f) / 2.0f);
    }
}
