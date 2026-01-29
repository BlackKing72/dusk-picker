using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Black.DuskPicker;

[StructLayout(LayoutKind.Sequential, Size = 32)]
public readonly struct ColorPacked(string colorName, byte r, byte g, byte b, byte a = 255)
{
    public static readonly ColorPacked White = new("White", 255, 255, 255);
    public static readonly ColorPacked Black = new("White", 0, 0, 0);

    public readonly ColorPackedName Name { get; init; } = new ColorPackedName(colorName);
    public readonly uint Color { get; init; } = PackColor(r, g, b, a);

    public readonly byte A => (byte)((Color >> 24) & 0xFF);
    public readonly byte R => (byte)((Color >> 16) & 0xFF);
    public readonly byte G => (byte)((Color >> 8) & 0xFF);
    public readonly byte B => (byte)((Color >> 0) & 0xFF);

    public static uint PackColor(byte r, byte g, byte b, byte a = 255)
    {
        return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
    }

    public static (byte a, byte r, byte g, byte b) UnpackColor(uint color)
    {
        return (
            a: (byte)((color >> 24) & 0xFF),
            r: (byte)((color >> 16) & 0xFF),
            g: (byte)((color >> 8) & 0xFF),
            b: (byte)((color >> 0) & 0xFF)
        );
    }

    public override string ToString()
    {
        return $"R: {R}, G: {G}, B: {B}";
    }
}

[InlineArray(MaxNameLength)]
public struct ColorPackedName
{
    public const int MaxNameLength = 28;

    private byte element;

    public Span<byte> Buffer => MemoryMarshal.CreateSpan(ref element, MaxNameLength);

    public ColorPackedName(string name)
        : this(Encoding.UTF8.GetBytes(name)) { }

    public ColorPackedName(ReadOnlySpan<byte> name)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, MaxNameLength, nameof(name));

        Buffer.Clear();
        name.CopyTo(Buffer);
    }

    public override string ToString()
    {
        int len = Buffer.IndexOf((byte)0);
        if (len < 0)
        {
            len = Buffer.Length;
        }

        return Encoding.UTF8.GetString(Buffer[..len]);
    }

    public static implicit operator string(ColorPackedName name)
    {
        return name.ToString();
    }
}
