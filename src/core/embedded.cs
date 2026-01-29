using System.Reflection;

namespace Black.DuskPicker;

public static class Embedded
{
    /// <summary>
    /// The path to an embedded resource will be the root namespace name if its
    /// present or the assembly/executable name. This assumes that root namespace
    /// are not set and the embedded resources used the executable name.
    /// </summary>
    /// <remarks>
    /// If you are not sure about what the path will be. You can list all
    /// embedded resources using:
    /// <code>
    ///     var assembly = Assembly.GetExecutingAssembly();
    ///     foreach (var name in assembly.GetManifestResourceNames())
    ///         Console.WriteLine($"resource: {name}");
    /// </code>
    /// </remarks>
    public static readonly string RootPath =
        Path.GetFileName(Environment.ProcessPath)
        ?? throw new ArgumentNullException(
            nameof(RootPath),
            "Can't find the root path for loading embedded resources."
        );

    /// <summary> Reads all bytes from a resource. </summary>
    /// <param name="resourcePath"> A relative path to the resource eg: <c>path/to/resource</c>. </param>
    public static byte[] ReadBytes(string resourcePath)
    {
        using Stream stream = ReadInternal(Assembly.GetExecutingAssembly(), resourcePath);
        byte[] buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);
        return buffer;
    }

    /// <summary> Reads all bytes from a resource asynchronously. </summary>
    /// <param name="resourcePath"> A relative path to the resource eg: <c>path/to/resource</c>. </param>
    public static async Task<byte[]> ReadBytesAsync(string resourcePath)
    {
        using Stream stream = ReadInternal(Assembly.GetExecutingAssembly(), resourcePath);
        byte[] buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);
        return buffer;
    }

    /// <summary> Interpret the resource as a string, reads all characters and returns them as one string. </summary>
    /// <param name="resourcePath"> A relative path to the resource eg: <c>path/to/resource</c>. </param>
    public static string ReadText(string resourcePath)
    {
        using StreamReader stream = new(
            ReadInternal(Assembly.GetExecutingAssembly(), resourcePath)
        );

        return stream.ReadToEnd();
    }

    /// <summary> Interpret the resource as a string, reads all characters asynchronously and returns them as one string. </summary>
    /// <param name="resourcePath"> A relative path to the resource eg: <c>path/to/resource</c>. </param>
    public static async Task<string> ReadTextAsync(string resourcePath)
    {
        using StreamReader stream = new(
            ReadInternal(Assembly.GetExecutingAssembly(), resourcePath)
        );

        return await stream.ReadToEndAsync();
    }

    public static Stream Read(string resourcePath)
    {
        return ReadInternal(Assembly.GetExecutingAssembly(), resourcePath);
    }

    /// <summary> Transforms a path e.g: <c>path/to/file</c> into a embedded resource path e.g: <c>path.to.file</c>. </summary>
    /// <param name="relativePath">A relative path to a embedded asset eg: <c>path/to/file</c> or <c>path\to\file</c>.</param>
    /// <returns>A new <c>string</c> containing the embedded path</returns>
    public static string ToEmbeddedPath(string relativePath)
    {
        relativePath = relativePath.Replace('/', '.').Replace('\\', '.');
        return $"{RootPath}.{relativePath}";
    }

    /// <summary> Tries to access a embedded resource inside an assembly. </summary>
    /// <param name="assembly"> Assembly that contains the resource. </param>
    /// <param name="resourcePath"> A relative path to the resource eg: <c>path/to/resource</c>. </param>
    /// <returns> A stream to the resource </returns>
    /// <exception cref="FileLoadException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private static Stream ReadInternal(Assembly assembly, string resourcePath)
    {
        string embeddedPath = ToEmbeddedPath(resourcePath);

        // Todo: Add logging back
        // Logr.Trace($"Resources: {resourcePath}");

        return assembly.GetManifestResourceStream(embeddedPath)
            ?? throw new FileLoadException(resourcePath);
    }
}
