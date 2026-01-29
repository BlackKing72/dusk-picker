using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public class BackgroundLayer(ScreenshotProviderLayer screenshotProvider) : Layer
{
    public override void OnRender()
    {
        Texture2D screenTexture = screenshotProvider.ScreenTexture;

        Rectangle textureRect = new(Vector2.Zero, screenTexture.Dimensions);
        Rectangle screenRect = new(Vector2.Zero, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        Raylib.DrawTexturePro(screenTexture, textureRect, screenRect, Vector2.Zero, 0, Color.White);
    }
}
