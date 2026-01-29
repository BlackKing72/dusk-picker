using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Black.DuskPicker;

public static class SpanExtensions
{
    extension<T>(Span<T> span) where T : unmanaged
    {
        public unsafe T* AsPtr()
        {
            return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        }
    }
}

public static class ReadOnlySpanExtensions
{
    extension<T>(ReadOnlySpan<T> span) where T : unmanaged
    {
        public unsafe T* AsPtr()
        {
            return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        }
    }
}

public static class MathExtensions
{
    extension(MathF)
    {
        public static float Lerp(float from, float to, float time)
        {
            return (1.0f - time) * from + to * time;
        }

        public static float InverseLerp(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        public static float Remap(float inMin, float inMax, float outMin, float outMax, float value)
        {
            float t = InverseLerp(inMin, inMax, value);
            return Lerp(outMin, outMax, t);
        }
    }
}

public static class Vector2Extensions
{
    extension(Vector2 vector)
    {
        public Vector2 Lerp(Vector2 to, float time)
        {
            return new(
                MathF.Lerp(vector.X, to.X, time),
                MathF.Lerp(vector.Y, to.Y, time));
        }


        public Vector2 Floor()
        {
            return new(
                MathF.Floor(vector.X),
                MathF.Floor(vector.Y));
        }

        public Vector2 Clamp(Vector2 min, Vector2 max)
        {
            return new(
                Math.Clamp(vector.X, min.X, max.X),
                Math.Clamp(vector.Y, min.Y, max.Y));
        }
    }
}
