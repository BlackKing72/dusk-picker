using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public class MagnifierLayer : Layer
{
    private readonly Shader magnifierShader;
    private readonly int uniformMousePosition;
    private readonly int uniformTextureSize;
    private readonly int uniformRadius;
    private readonly int uniformZoom;

    private Vector2 rawMousePosition = Vector2.Zero;
    private Vector2 localMousePosition = Vector2.Zero;
    private Vector2 texelSnappedMousePosition;
    private Vector2 localSnappedMousePosition = Vector2.Zero;

    private float mouseSensitivity = 1.0f;

    private float magnifierZoom = 2.0f;
    private float magnifierRadius = 50.0f;

    private readonly StateProvider stateProvider;
    private readonly ScreenshotProvider screenshotProvider;
    public readonly PickerOptions options;

    public MagnifierLayer(
        StateProvider stateProvider,
        ScreenshotProvider screenshotProvider,
        PickerOptions? options = null
    )
    {
        this.stateProvider = stateProvider;
        this.screenshotProvider = screenshotProvider;

        this.options = options ?? new PickerOptions();

        magnifierShader = LoadShader("assets/shaders/magnifier.fs");
        uniformZoom = Raylib.GetShaderLocation(magnifierShader, "u_zoom");
        uniformRadius = Raylib.GetShaderLocation(magnifierShader, "u_radius");
        uniformTextureSize = Raylib.GetShaderLocation(magnifierShader, "u_textureSize");
        uniformMousePosition = Raylib.GetShaderLocation(magnifierShader, "u_mousePosition");
    }

    public override void OnEvent(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowOpenEvent>(OnWindowOpen);
    }

    public override bool OnUpdate()
    {
        Texture2D screenTexture = screenshotProvider.ScreenTexture;
        Image screenImage = screenshotProvider.ScreenImage;

        Vector2 screenDimensions = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Rectangle srcRect = new(Vector2.Zero, screenTexture.Dimensions);
        Rectangle dstRect = new(Vector2.Zero, screenDimensions);

        mouseSensitivity = MathF.Lerp(
            options.MoveSensitivityAtMinZoom,
            options.MoveSensitivityAtMaxZoom,
            Easings.EaseInOutQuart(
                MathF.InverseLerp(options.MinMagnifierZoom, options.MaxMagnifierZoom, magnifierZoom)
            )
        );

        rawMousePosition += Raylib.GetMouseDelta() * mouseSensitivity;
        rawMousePosition = rawMousePosition.Clamp(Vector2.Zero, screenDimensions);

        bool isShiftDown =
            Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

        Vector2 scale = srcRect.Size / dstRect.Size;

        localMousePosition = rawMousePosition - dstRect.Position;
        Vector2 texelMousePosition = localMousePosition * scale;
        texelSnappedMousePosition = new(
            MathF.Floor(texelMousePosition.X) + 0.5f,
            MathF.Floor(texelMousePosition.Y) + 0.5f
        );

        localSnappedMousePosition = texelSnappedMousePosition / scale;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Color color = screenshotProvider.GetColorAt(
                (int)texelSnappedMousePosition.X,
                (int)texelSnappedMousePosition.Y
            );

            stateProvider.SelectColor(color, localSnappedMousePosition);
        }

        float scrollDelta = Raylib.GetMouseWheelMove();
        float deltaTime = Raylib.GetFrameTime();

        if (isShiftDown)
        {
            magnifierRadius += scrollDelta * options.RadiusSensitivity * deltaTime;
            magnifierRadius = Math.Clamp(
                magnifierRadius,
                options.MinMagnifierRadius,
                options.MaxMagnifierRadius
            );
        }
        else
        {
            magnifierZoom += scrollDelta * options.ZoomSensitivity * deltaTime;
            magnifierZoom = Math.Clamp(
                magnifierZoom,
                options.MinMagnifierZoom,
                options.MaxMagnifierZoom
            );
        }

        return false;
    }

    public override void OnRender()
    {
        Texture2D screenTexture = screenshotProvider.ScreenTexture;

        Rectangle srcRect = new(Vector2.Zero, screenTexture.Width, screenTexture.Height);
        Rectangle dstRect = new(Vector2.Zero, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Vector2 scale = srcRect.Size / dstRect.Size;

        // Draw Magnifying Glass Effect
        Raylib.BeginShaderMode(magnifierShader);

        Raylib.SetShaderValue(
            magnifierShader,
            uniformMousePosition,
            texelSnappedMousePosition,
            ShaderUniformDataType.Vec2
        );
        Raylib.SetShaderValue(
            magnifierShader,
            uniformTextureSize,
            screenTexture.Dimensions,
            ShaderUniformDataType.Vec2
        );
        Raylib.SetShaderValue(
            magnifierShader,
            uniformRadius,
            magnifierRadius,
            ShaderUniformDataType.Float
        );
        Raylib.SetShaderValue(
            magnifierShader,
            uniformZoom,
            magnifierZoom,
            ShaderUniformDataType.Float
        );

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

    private static unsafe Shader LoadShader(string relativePath)
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
