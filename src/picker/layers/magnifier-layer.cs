using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public class MagnifierLayer : Layer
{
    private readonly StateProvider stateProvider;
    private readonly ScreenshotProvider screenshotProvider;
    private readonly PickerOptions options;

    private readonly Shader magnifierShader;
    private readonly ShaderUniform uniformMousePosition = new("u_mousePosition");
    private readonly ShaderUniform uniformTextureSize = new("u_textureSize");
    private readonly ShaderUniform uniformRadius = new("u_radius");
    private readonly ShaderUniform uniformZoom = new("u_zoom");

    private Vector2 rawMousePosition = Vector2.Zero;
    private Vector2 texelSnappedMousePosition; // the mouse position in relation to the zoomed texture.
    private Vector2 localSnappedMousePosition = Vector2.Zero; // the mouse position in relation to the current window.
    private float magnifierZoom = 2.0f;
    private float magnifierRadius = 50.0f;

    public MagnifierLayer(
        StateProvider stateProvider,
        ScreenshotProvider screenshotProvider,
        PickerOptions? options = null
    )
    {
        this.stateProvider = stateProvider;
        this.screenshotProvider = screenshotProvider;

        this.options = options ?? new PickerOptions();

        magnifierShader = LoadEmbeddedShader("assets/shaders/magnifier.fs");
    }

    public override void OnEvent(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowOpenEvent>(OnWindowOpen);
    }

    public override void OnRender()
    {
        Texture2D screenTexture = screenshotProvider.ScreenTexture;

        Rectangle srcRect = new(Vector2.Zero, screenTexture.Width, screenTexture.Height);
        Rectangle dstRect = new(Vector2.Zero, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Vector2 scale = srcRect.Size / dstRect.Size;

        // Draw Magnifying Glass Effect
        Raylib.BeginShaderMode(magnifierShader);

        uniformMousePosition.SetValue(
            magnifierShader,
            texelSnappedMousePosition,
            ShaderUniformDataType.Vec2
        );
        uniformTextureSize.SetValue(
            magnifierShader,
            screenTexture.Dimensions,
            ShaderUniformDataType.Vec2
        );
        uniformRadius.SetValue(magnifierShader, magnifierRadius, ShaderUniformDataType.Float);
        uniformZoom.SetValue(magnifierShader, magnifierZoom, ShaderUniformDataType.Float);

        Raylib.DrawTexturePro(screenTexture, srcRect, dstRect, Vector2.Zero, 0, Color.White);

        Raylib.EndShaderMode();

        // Draw Magnifying Glass Border
        Raylib.DrawCircleLines(
            (int)localSnappedMousePosition.X,
            (int)localSnappedMousePosition.Y,
            magnifierRadius,
            Color.Gray
        );

        // Draw Pixel Crosshair
        Vector2 pixelSize = new Vector2(magnifierZoom) / scale;
        Rectangle crosshair = new(localSnappedMousePosition - (pixelSize * 0.5f), pixelSize);

        Raylib.DrawRectangleLinesEx(crosshair, 1.5f, Color.Gray);
    }

    public override bool OnUpdate()
    {
        HandleMouseMovement();
        HandleMouseInput();

        return false;
    }

    private void HandleMouseMovement()
    {
        Texture2D screenTexture = screenshotProvider.ScreenTexture;
        Vector2 screenDimensions = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Rectangle srcRect = new(Vector2.Zero, screenTexture.Dimensions);
        Rectangle dstRect = new(Vector2.Zero, screenDimensions);

        float mouseSensitivity = MathF.Lerp(
            options.MoveSensitivityAtMinZoom,
            options.MoveSensitivityAtMaxZoom,
            Easings.EaseInOutQuart(
                MathF.InverseLerp(options.MinMagnifierZoom, options.MaxMagnifierZoom, magnifierZoom)
            )
        );

        rawMousePosition += Raylib.GetMouseDelta() * mouseSensitivity;
        rawMousePosition = rawMousePosition.Clamp(Vector2.Zero, screenDimensions);

        Vector2 scale = srcRect.Size / dstRect.Size;

        Vector2 localMousePosition = rawMousePosition - dstRect.Position;
        Vector2 texelMousePosition = localMousePosition * scale;
        texelSnappedMousePosition = new(
            MathF.Floor(texelMousePosition.X) + 0.5f,
            MathF.Floor(texelMousePosition.Y) + 0.5f
        );

        localSnappedMousePosition = texelSnappedMousePosition / scale;

        stateProvider.MouseTexPosition = texelSnappedMousePosition;
        stateProvider.MousePosition = localSnappedMousePosition;
    }

    private void HandleMouseInput()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Color color = screenshotProvider.GetColorAt(
                (int)texelSnappedMousePosition.X,
                (int)texelSnappedMousePosition.Y
            );

            stateProvider.SelectColor(color, localSnappedMousePosition);
        }

        float scrollDelta = Raylib.GetMouseWheelMove();

        if (Raylib.IsKeyDown(KeyboardKey.LeftShift, KeyboardKey.RightShift))
        {
            magnifierRadius += scrollDelta * options.RadiusSensitivity;
            magnifierRadius = Math.Clamp(
                magnifierRadius,
                options.MinMagnifierRadius,
                options.MaxMagnifierRadius
            );
        }
        else
        {
            magnifierZoom += scrollDelta * options.ZoomSensitivity;
            magnifierZoom = Math.Clamp(
                magnifierZoom,
                options.MinMagnifierZoom,
                options.MaxMagnifierZoom
            );
        }
    }

    private static unsafe Shader LoadEmbeddedShader(string relativePath)
    {
        string shaderPath = Embedded.ToEmbeddedPath(relativePath);
        Raylib.TraceLog(TraceLogLevel.Info, $"MAGNIFIER: Loading shader at: '{shaderPath}'");
        byte[] buffer = Embedded.ReadBytes(relativePath);
        return Raylib.LoadShaderFromMemory(null, (sbyte*)buffer.AsPtr());
    }

    private void OnWindowOpen(WindowOpenEvent evt)
    {
        rawMousePosition = Raylib.GetMousePosition();
    }
}
