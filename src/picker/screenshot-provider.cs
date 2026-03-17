using Raylib_cs;
using Shutter;

namespace Black.DuskPicker;

public class ScreenshotProvider
{
    public Texture2D ScreenTexture { get; private set; }
    public Image ScreenImage { get; private set; }

    private bool isDirty = false;

    public void OnUpdate()
    {
        if (isDirty)
        {
            isDirty = false;

            // ideally the texture should be updated only when the window open,
            // so the texture should be already unloaded at this point, but in case it's not.
            if (Raylib.IsTextureValid(ScreenTexture))
                Raylib.UnloadTexture(ScreenTexture);

            ScreenTexture = Raylib.LoadTextureFromImage(ScreenImage);
            Raylib.SetTextureFilter(ScreenTexture, TextureFilter.Point);
            Raylib.SetTextureWrap(ScreenTexture, TextureWrap.Clamp);
        }
    }

    public void OnEvent(Event evt)
    {
        EventDispatcher dispatcher = new(evt);
        dispatcher.Dispatch<WindowOpenEvent>(OnWindowOpen);
        dispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
    }

    public Color GetColorAt(int x, int y)
    {
        var w = ScreenImage.Width;
        var h = ScreenImage.Height;

        bool isInsideBounds = x >= 0 && y >= 0 && x <= w && y <= h;

        return Raylib.IsImageValid(ScreenImage) && isInsideBounds
            ? Raylib.GetImageColor(ScreenImage, x, y)
            : Color.RayWhite;
    }

    private void OnWindowOpen(WindowOpenEvent evt)
    {
        // take a new screenshot and load it into an image.
        ShutterService screenshot = new();
        byte[] screenData = screenshot.TakeScreenshot();
        ScreenImage = Raylib.LoadImageFromMemory(".png", screenData);

        // the event it's not called in sync with the raylib, so when this is called
        // the window don't have a valid opengl context (at least in linux/x11),
        // so it's not able to create a texture in the event callback. the workaround
        // is to mark the image as "dirt" and update the texture in the next frame.
        isDirty = true;
    }

    private void OnWindowClose(WindowCloseEvent evt)
    {
        // unload image and texture to free up memory while closed.
        Raylib.UnloadImage(ScreenImage);
        Raylib.UnloadTexture(ScreenTexture);
    }
}
