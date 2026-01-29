using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public static class RectangleX
{
    extension(Rectangle rect)
    {
        public bool IsInside(Vector2 point)
        {
            return point.X >= rect.X
                && point.Y >= rect.Y
                && point.X <= rect.X + rect.Width
                && point.Y <= rect.Y + rect.Height;
        }
    }
}
