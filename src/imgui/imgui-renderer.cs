using System.Numerics;
using System.Runtime.InteropServices;
using Hexa.NET.ImGui;
using Raylib_cs;
using ImDrawIdx = ushort;

namespace Black.DuskPicker.ImRaylib;

public static unsafe class GuiRenderer
{
    private static ImGuiMouseCursor CurrentMouseCursor = ImGuiMouseCursor.Count;
    private static readonly MouseCursor[] MouseCursorMap = new MouseCursor[
        (int)ImGuiMouseCursor.Count
    ];

    private static bool LastFrameFocused;
    private static bool LastControlPressed;
    private static bool LastShiftPressed;
    private static bool LastAltPressed;
    private static bool LastSuperPressed;

    // internal only functions
    private static bool RlImGuiIsControlDown()
    {
        return Raylib.IsKeyDown(KeyboardKey.RightControl)
            || Raylib.IsKeyDown(KeyboardKey.LeftControl);
    }

    private static bool RlImGuiIsShiftDown()
    {
        return Raylib.IsKeyDown(KeyboardKey.RightShift) || Raylib.IsKeyDown(KeyboardKey.LeftShift);
    }

    private static bool RlImGuiIsAltDown()
    {
        return Raylib.IsKeyDown(KeyboardKey.RightAlt) || Raylib.IsKeyDown(KeyboardKey.LeftAlt);
    }

    private static bool RlImGuiIsSuperDown()
    {
        return Raylib.IsKeyDown(KeyboardKey.RightSuper) || Raylib.IsKeyDown(KeyboardKey.LeftSuper);
    }

    private static void ReloadFonts()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        byte* pixels;

        int width;
        int height;
        io.Fonts.GetTexDataAsRGBA32(&pixels, &width, &height, null);

        Image image = new()
        {
            Data = pixels,
            Width = width,
            Height = height,
            Mipmaps = 1,
            Format = PixelFormat.UncompressedR8G8B8A8,
        };

        Texture2D* fontTexture = (Texture2D*)io.Fonts.TexID.Handle;
        if (fontTexture != null && fontTexture->Id != 0)
        {
            Raylib.UnloadTexture(*fontTexture);
            Raylib.MemFree(fontTexture);
        }

        fontTexture = (Texture2D*)Raylib.MemAlloc((uint)sizeof(Texture2D));
        *fontTexture = Raylib.LoadTextureFromImage(image);

        io.Fonts.TexID = new ImTextureID((nint)fontTexture);
    }

    private static byte* GetClipTextCallback(ImGuiContext* ctx)
    {
        return (byte*)Raylib.GetClipboardText();
    }

    private static void SetClipTextCallback(ImGuiContext* ctx, byte* text)
    {
        Raylib.SetClipboardText((sbyte*)text);
    }

    private static void ImGuiNewFrame(float deltaTime)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        Vector2 resolutionScale = Raylib.GetWindowScaleDPI();

#if !PLATFORM_DRM
        if (Raylib.IsWindowFullscreen())
        {
            int monitor = Raylib.GetCurrentMonitor();
            io.DisplaySize.X = (Raylib.GetMonitorWidth(monitor));
            io.DisplaySize.Y = (Raylib.GetMonitorHeight(monitor));
        }
        else
        {
            io.DisplaySize.X = (Raylib.GetScreenWidth());
            io.DisplaySize.Y = (Raylib.GetScreenHeight());
        }

#if !APPLE
        if (!Raylib.IsWindowState(ConfigFlags.HighDpiWindow))
        {
            resolutionScale = new Vector2(1, 1);
        }
#endif
#else
        io.DisplaySize.X = (Raylib.GetScreenWidth());
        io.DisplaySize.Y = (Raylib.GetScreenHeight());
#endif

        io.DisplayFramebufferScale = new(resolutionScale.X, resolutionScale.Y);

        io.DeltaTime = deltaTime;

        if (io.WantSetMousePos)
        {
            Raylib.SetMousePosition((int)io.MousePos.X, (int)io.MousePos.Y);
        }
        else
        {
            io.AddMousePosEvent((float)Raylib.GetMouseX(), (float)Raylib.GetMouseY());
        }

        static void setMouseEvent(ImGuiIOPtr io, MouseButton rayMouse, ImGuiMouseButton imGuiMouse)
        {
            if (Raylib.IsMouseButtonPressed(rayMouse))
            {
                io.AddMouseButtonEvent((int)imGuiMouse, true);
            }
            else if (Raylib.IsMouseButtonReleased(rayMouse))
            {
                io.AddMouseButtonEvent((int)imGuiMouse, false);
            }
        }
        ;

        setMouseEvent(io, MouseButton.Left, ImGuiMouseButton.Left);
        setMouseEvent(io, MouseButton.Right, ImGuiMouseButton.Right);
        setMouseEvent(io, MouseButton.Middle, ImGuiMouseButton.Middle);
        setMouseEvent(io, MouseButton.Forward, (ImGuiMouseButton.Middle + 1));
        setMouseEvent(io, MouseButton.Back, (ImGuiMouseButton.Middle + 2));

        {
            Vector2 mouseWheel = Raylib.GetMouseWheelMoveV();
            io.AddMouseWheelEvent(mouseWheel.X, mouseWheel.Y);
        }

        if ((ImGui.GetIO().BackendFlags & ImGuiBackendFlags.HasMouseCursors) != 0)
        {
            if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == 0)
            {
                ImGuiMouseCursor imguiCursor = ImGui.GetMouseCursor();
                if (imguiCursor != CurrentMouseCursor || io.MouseDrawCursor)
                {
                    CurrentMouseCursor = imguiCursor;
                    if (io.MouseDrawCursor || imguiCursor == ImGuiMouseCursor.None)
                    {
                        Raylib.HideCursor();
                    }
                    else
                    {
                        Raylib.ShowCursor();

                        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == 0)
                        {
                            Raylib.SetMouseCursor(
                                ((int)imguiCursor > -1 && imguiCursor < ImGuiMouseCursor.Count)
                                    ? MouseCursorMap[(int)imguiCursor]
                                    : MouseCursor.Default
                            );
                        }
                    }
                }
            }
        }
    }

    private static void ImGuiTriangleVert(ImDrawVert idx_vert)
    {
        Color* c = (Color*)&idx_vert.Col;
        Rlgl.Color4ub(c->R, c->G, c->B, c->A);
        Rlgl.TexCoord2f(idx_vert.Uv.X, idx_vert.Uv.Y);
        Rlgl.Vertex2f(idx_vert.Pos.X, idx_vert.Pos.Y);
    }

    private static void ImGuiRenderTriangles(
        uint count,
        uint indexStart,
        ImVector<ImDrawIdx>* indexBuffer,
        ImVector<ImDrawVert>* vertBuffer,
        void* texturePtr
    )
    {
        if (count < 3)
        {
            return;
        }

        Texture2D* texture = (Texture2D*)texturePtr;
        uint textureId = (texture == null) ? 0 : texture->Id;

        Rlgl.Begin(DrawMode.Triangles);
        Rlgl.SetTexture(textureId);

        for (uint i = 0; i <= (count - 3); i += 3)
        {
            ImDrawIdx indexA = indexBuffer->Data[indexStart + i];
            ImDrawIdx indexB = indexBuffer->Data[indexStart + i + 1];
            ImDrawIdx indexC = indexBuffer->Data[indexStart + i + 2];

            ImDrawVert vertexA = vertBuffer->Data[indexA];
            ImDrawVert vertexB = vertBuffer->Data[indexB];
            ImDrawVert vertexC = vertBuffer->Data[indexC];

            ImGuiTriangleVert(vertexA);
            ImGuiTriangleVert(vertexB);
            ImGuiTriangleVert(vertexC);
        }

        Rlgl.End();
    }

    private static void EnableScissor(float x, float y, float width, float height)
    {
        Rlgl.EnableScissorTest();
        ImGuiIOPtr io = ImGui.GetIO();

        Vector2 scale = io.DisplayFramebufferScale;
#if !APPLE
        if (!Raylib.IsWindowState(ConfigFlags.HighDpiWindow))
        {
            scale.X = 1;
            scale.Y = 1;
        }
#endif

        Rlgl.Scissor(
            (int)(x * scale.X),
            (int)((io.DisplaySize.Y - (int)(y + height)) * scale.Y),
            (int)(width * scale.X),
            (int)(height * scale.Y)
        );
    }

    private static void SetupMouseCursors()
    {
        MouseCursorMap[(int)ImGuiMouseCursor.Arrow] = MouseCursor.Arrow;
        MouseCursorMap[(int)ImGuiMouseCursor.TextInput] = MouseCursor.IBeam;
        MouseCursorMap[(int)ImGuiMouseCursor.Hand] = MouseCursor.PointingHand;
        MouseCursorMap[(int)ImGuiMouseCursor.ResizeAll] = MouseCursor.ResizeAll;
        MouseCursorMap[(int)ImGuiMouseCursor.ResizeEw] = MouseCursor.ResizeEw;
        MouseCursorMap[(int)ImGuiMouseCursor.ResizeNesw] = MouseCursor.ResizeNesw;
        MouseCursorMap[(int)ImGuiMouseCursor.ResizeNs] = MouseCursor.ResizeNs;
        MouseCursorMap[(int)ImGuiMouseCursor.ResizeNwse] = MouseCursor.ResizeNwse;
        MouseCursorMap[(int)ImGuiMouseCursor.NotAllowed] = MouseCursor.NotAllowed;
    }

    private static ImGuiKey MapKeyToImGuiKey(KeyboardKey key)
    {
        return key switch
        {
            KeyboardKey.Apostrophe => ImGuiKey.Apostrophe,
            KeyboardKey.Comma => ImGuiKey.Comma,
            KeyboardKey.Minus => ImGuiKey.Minus,
            KeyboardKey.Period => ImGuiKey.Period,
            KeyboardKey.Slash => ImGuiKey.Slash,
            KeyboardKey.Zero => ImGuiKey.Key0,
            KeyboardKey.One => ImGuiKey.Key1,
            KeyboardKey.Two => ImGuiKey.Key2,
            KeyboardKey.Three => ImGuiKey.Key3,
            KeyboardKey.Four => ImGuiKey.Key4,
            KeyboardKey.Five => ImGuiKey.Key5,
            KeyboardKey.Six => ImGuiKey.Key6,
            KeyboardKey.Seven => ImGuiKey.Key7,
            KeyboardKey.Eight => ImGuiKey.Key8,
            KeyboardKey.Nine => ImGuiKey.Key9,
            KeyboardKey.Semicolon => ImGuiKey.Semicolon,
            KeyboardKey.Equal => ImGuiKey.Equal,
            KeyboardKey.A => ImGuiKey.A,
            KeyboardKey.B => ImGuiKey.B,
            KeyboardKey.C => ImGuiKey.C,
            KeyboardKey.D => ImGuiKey.D,
            KeyboardKey.E => ImGuiKey.E,
            KeyboardKey.F => ImGuiKey.F,
            KeyboardKey.G => ImGuiKey.G,
            KeyboardKey.H => ImGuiKey.H,
            KeyboardKey.I => ImGuiKey.I,
            KeyboardKey.J => ImGuiKey.J,
            KeyboardKey.K => ImGuiKey.K,
            KeyboardKey.L => ImGuiKey.L,
            KeyboardKey.M => ImGuiKey.M,
            KeyboardKey.N => ImGuiKey.N,
            KeyboardKey.O => ImGuiKey.O,
            KeyboardKey.P => ImGuiKey.P,
            KeyboardKey.Q => ImGuiKey.Q,
            KeyboardKey.R => ImGuiKey.R,
            KeyboardKey.S => ImGuiKey.S,
            KeyboardKey.T => ImGuiKey.T,
            KeyboardKey.U => ImGuiKey.U,
            KeyboardKey.V => ImGuiKey.V,
            KeyboardKey.W => ImGuiKey.W,
            KeyboardKey.X => ImGuiKey.X,
            KeyboardKey.Y => ImGuiKey.Y,
            KeyboardKey.Z => ImGuiKey.Z,
            KeyboardKey.Space => ImGuiKey.Space,
            KeyboardKey.Escape => ImGuiKey.Escape,
            KeyboardKey.Enter => ImGuiKey.Enter,
            KeyboardKey.Tab => ImGuiKey.Tab,
            KeyboardKey.Backspace => ImGuiKey.Backspace,
            KeyboardKey.Insert => ImGuiKey.Insert,
            KeyboardKey.Delete => ImGuiKey.Delete,
            KeyboardKey.Right => ImGuiKey.RightArrow,
            KeyboardKey.Left => ImGuiKey.LeftArrow,
            KeyboardKey.Down => ImGuiKey.DownArrow,
            KeyboardKey.Up => ImGuiKey.UpArrow,
            KeyboardKey.PageUp => ImGuiKey.PageUp,
            KeyboardKey.PageDown => ImGuiKey.PageDown,
            KeyboardKey.Home => ImGuiKey.Home,
            KeyboardKey.End => ImGuiKey.End,
            KeyboardKey.CapsLock => ImGuiKey.CapsLock,
            KeyboardKey.ScrollLock => ImGuiKey.ScrollLock,
            KeyboardKey.NumLock => ImGuiKey.NumLock,
            KeyboardKey.PrintScreen => ImGuiKey.PrintScreen,
            KeyboardKey.Pause => ImGuiKey.Pause,
            KeyboardKey.F1 => ImGuiKey.F1,
            KeyboardKey.F2 => ImGuiKey.F2,
            KeyboardKey.F3 => ImGuiKey.F3,
            KeyboardKey.F4 => ImGuiKey.F4,
            KeyboardKey.F5 => ImGuiKey.F5,
            KeyboardKey.F6 => ImGuiKey.F6,
            KeyboardKey.F7 => ImGuiKey.F7,
            KeyboardKey.F8 => ImGuiKey.F8,
            KeyboardKey.F9 => ImGuiKey.F9,
            KeyboardKey.F10 => ImGuiKey.F10,
            KeyboardKey.F11 => ImGuiKey.F11,
            KeyboardKey.F12 => ImGuiKey.F12,
            KeyboardKey.LeftShift => ImGuiKey.LeftShift,
            KeyboardKey.LeftControl => ImGuiKey.LeftCtrl,
            KeyboardKey.LeftAlt => ImGuiKey.LeftAlt,
            KeyboardKey.LeftSuper => ImGuiKey.LeftSuper,
            KeyboardKey.RightShift => ImGuiKey.RightShift,
            KeyboardKey.RightControl => ImGuiKey.RightCtrl,
            KeyboardKey.RightAlt => ImGuiKey.RightAlt,
            KeyboardKey.RightSuper => ImGuiKey.RightSuper,
            KeyboardKey.KeyboardMenu => ImGuiKey.Menu,
            KeyboardKey.LeftBracket => ImGuiKey.LeftBracket,
            KeyboardKey.Backslash => ImGuiKey.Backslash,
            KeyboardKey.RightBracket => ImGuiKey.RightBracket,
            KeyboardKey.Grave => ImGuiKey.GraveAccent,
            KeyboardKey.Kp0 => ImGuiKey.Keypad0,
            KeyboardKey.Kp1 => ImGuiKey.Keypad1,
            KeyboardKey.Kp2 => ImGuiKey.Keypad2,
            KeyboardKey.Kp3 => ImGuiKey.Keypad3,
            KeyboardKey.Kp4 => ImGuiKey.Keypad4,
            KeyboardKey.Kp5 => ImGuiKey.Keypad5,
            KeyboardKey.Kp6 => ImGuiKey.Keypad6,
            KeyboardKey.Kp7 => ImGuiKey.Keypad7,
            KeyboardKey.Kp8 => ImGuiKey.Keypad8,
            KeyboardKey.Kp9 => ImGuiKey.Keypad9,
            KeyboardKey.KpDecimal => ImGuiKey.KeypadDecimal,
            KeyboardKey.KpDivide => ImGuiKey.KeypadDivide,
            KeyboardKey.KpMultiply => ImGuiKey.KeypadMultiply,
            KeyboardKey.KpSubtract => ImGuiKey.KeypadSubtract,
            KeyboardKey.KpAdd => ImGuiKey.KeypadAdd,
            KeyboardKey.KpEnter => ImGuiKey.KeypadEnter,
            KeyboardKey.KpEqual => ImGuiKey.KeypadEqual,
            _ => ImGuiKey.None,
        };
    }

    // raw ImGui backend API
    public static bool Init()
    {
        LastFrameFocused = Raylib.IsWindowFocused();
        LastControlPressed = false;
        LastShiftPressed = false;
        LastAltPressed = false;
        LastSuperPressed = false;

        SetupMouseCursors();

        ImGuiIOPtr io = ImGui.GetIO();
        io.BackendPlatformName = "imgui_impl_raylib"u8.AsPtr();
        io.BackendFlags |= ImGuiBackendFlags.HasGamepad | ImGuiBackendFlags.HasSetMousePos;

#if !PLATFORM_DRM
        io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
#endif

        io.MousePos = new(0, 0);

        ImGuiPlatformIOPtr platformIO = ImGui.GetPlatformIO();

        platformIO.PlatformSetClipboardTextFn = (void*)
            Marshal.GetFunctionPointerForDelegate<PlatformSetClipboardTextFn>(SetClipTextCallback);
        platformIO.PlatformGetClipboardTextFn = (void*)
            Marshal.GetFunctionPointerForDelegate<PlatformGetClipboardTextFn>(GetClipTextCallback);

        platformIO.PlatformClipboardUserData = null;

        BuildFontAtlas();

        return true;
    }

    private static void BuildFontAtlas()
    {
        ReloadFonts();
    }

    public static void Shutdown()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        Texture2D* fontTexture = (Texture2D*)io.Fonts.TexID.Handle;

        if (fontTexture != null)
        {
            Raylib.UnloadTexture(*fontTexture);
            Raylib.MemFree(fontTexture);
        }

        io.Fonts.TexID = 0;
    }

    public static void NewFrame()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        ImGuiNewFrame(Raylib.GetFrameTime());
        _ = ProcessEvents();
    }

    public static readonly void* nullptr = (void*)0;

    public static unsafe void RenderDrawData(ImDrawData* drawData)
    {
        Rlgl.DrawRenderBatchActive();
        Rlgl.DisableBackfaceCulling();

        for (int l = 0; l < drawData->CmdListsCount; ++l)
        {
            ImDrawList* commandList = drawData->CmdLists[l];

            for (int i = 0; i < commandList->CmdBuffer.Size; i++)
            {
                ImDrawCmd cmd = commandList->CmdBuffer[i];
                EnableScissor(
                    cmd.ClipRect.X - drawData->DisplayPos.X,
                    cmd.ClipRect.Y - drawData->DisplayPos.Y,
                    cmd.ClipRect.Z - (cmd.ClipRect.X - drawData->DisplayPos.X),
                    cmd.ClipRect.W - (cmd.ClipRect.Y - drawData->DisplayPos.Y)
                );
                if (cmd.UserCallback != null)
                {
                    delegate* <ImDrawList*, ImDrawCmd*, void> userCallback = (delegate* <
                        ImDrawList*,
                        ImDrawCmd*,
                        void>)
                        cmd.UserCallback;

                    userCallback(commandList, &cmd);

                    continue;
                }

                ImGuiRenderTriangles(
                    cmd.ElemCount,
                    cmd.IdxOffset,
                    &commandList->IdxBuffer,
                    &commandList->VtxBuffer,
                    (void*)cmd.TextureId.Handle
                );
                Rlgl.DrawRenderBatchActive();
            }
        }

        Rlgl.SetTexture(0);
        Rlgl.DisableScissorTest();
        Rlgl.EnableBackfaceCulling();
    }

    private static void HandleGamepadButtonEvent(ImGuiIOPtr io, GamepadButton button, ImGuiKey key)
    {
        if (Raylib.IsGamepadButtonPressed(0, button))
        {
            io.AddKeyEvent(key, true);
        }
        else if (Raylib.IsGamepadButtonReleased(0, button))
        {
            io.AddKeyEvent(key, false);
        }
    }

    private static void HandleGamepadStickEvent(
        ImGuiIOPtr io,
        GamepadAxis axis,
        ImGuiKey negKey,
        ImGuiKey posKey
    )
    {
        const float deadZone = 0.20f;

        float axisValue = Raylib.GetGamepadAxisMovement(0, axis);

        io.AddKeyAnalogEvent(negKey, axisValue < -deadZone, axisValue < -deadZone ? -axisValue : 0);
        io.AddKeyAnalogEvent(posKey, axisValue > deadZone, axisValue > deadZone ? axisValue : 0);
    }

    private static bool ProcessEvents()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        bool focused = Raylib.IsWindowFocused();
        if (focused != LastFrameFocused)
        {
            io.AddFocusEvent(focused);
        }

        LastFrameFocused = focused;

        // handle the modifyer key events so that shortcuts work
        bool ctrlDown = RlImGuiIsControlDown();
        if (ctrlDown != LastControlPressed)
        {
            io.AddKeyEvent(ImGuiKey.ModCtrl, ctrlDown);
        }

        LastControlPressed = ctrlDown;

        bool shiftDown = RlImGuiIsShiftDown();
        if (shiftDown != LastShiftPressed)
        {
            io.AddKeyEvent(ImGuiKey.ModShift, shiftDown);
        }

        LastShiftPressed = shiftDown;

        bool altDown = RlImGuiIsAltDown();
        if (altDown != LastAltPressed)
        {
            io.AddKeyEvent(ImGuiKey.ModAlt, altDown);
        }

        LastAltPressed = altDown;

        bool superDown = RlImGuiIsSuperDown();
        if (superDown != LastSuperPressed)
        {
            io.AddKeyEvent(ImGuiKey.ModSuper, superDown);
        }

        LastSuperPressed = superDown;

        // get the pressed keys, just walk the keys so we don
        for (int keyId = (int)KeyboardKey.Null; keyId < (int)KeyboardKey.KpEqual; keyId++)
        {
            if (Raylib.IsKeyReleased((KeyboardKey)keyId))
            {
                io.AddKeyEvent(MapKeyToImGuiKey((KeyboardKey)keyId), false);
            }
            else if (Raylib.IsKeyPressed((KeyboardKey)keyId))
            {
                io.AddKeyEvent(MapKeyToImGuiKey((KeyboardKey)keyId), true);
            }
        }

        if (io.WantCaptureKeyboard)
        {
            // add the text input in order
            uint pressed = (uint)Raylib.GetCharPressed();
            while (pressed != 0)
            {
                io.AddInputCharacter(pressed);
                pressed = (uint)Raylib.GetCharPressed();
            }
        }

        if (
            (io.ConfigFlags & ImGuiConfigFlags.NavEnableGamepad) != 0
            && Raylib.IsGamepadAvailable(0)
        )
        {
            HandleGamepadButtonEvent(io, GamepadButton.LeftFaceUp, ImGuiKey.GamepadDpadUp);
            HandleGamepadButtonEvent(io, GamepadButton.LeftFaceRight, ImGuiKey.GamepadDpadRight);
            HandleGamepadButtonEvent(io, GamepadButton.LeftFaceDown, ImGuiKey.GamepadDpadDown);
            HandleGamepadButtonEvent(io, GamepadButton.LeftFaceLeft, ImGuiKey.GamepadDpadLeft);

            HandleGamepadButtonEvent(io, GamepadButton.RightFaceUp, ImGuiKey.GamepadFaceUp);
            HandleGamepadButtonEvent(io, GamepadButton.RightFaceRight, ImGuiKey.GamepadFaceLeft);
            HandleGamepadButtonEvent(io, GamepadButton.RightFaceDown, ImGuiKey.GamepadFaceDown);
            HandleGamepadButtonEvent(io, GamepadButton.RightFaceLeft, ImGuiKey.GamepadFaceRight);

            HandleGamepadButtonEvent(io, GamepadButton.LeftTrigger1, ImGuiKey.GamepadL1);
            HandleGamepadButtonEvent(io, GamepadButton.LeftTrigger2, ImGuiKey.GamepadL2);
            HandleGamepadButtonEvent(io, GamepadButton.RightTrigger1, ImGuiKey.GamepadR1);
            HandleGamepadButtonEvent(io, GamepadButton.RightTrigger2, ImGuiKey.GamepadR2);
            HandleGamepadButtonEvent(io, GamepadButton.LeftThumb, ImGuiKey.GamepadL3);
            HandleGamepadButtonEvent(io, GamepadButton.RightThumb, ImGuiKey.GamepadR3);

            HandleGamepadButtonEvent(io, GamepadButton.MiddleLeft, ImGuiKey.GamepadStart);
            HandleGamepadButtonEvent(io, GamepadButton.MiddleRight, ImGuiKey.GamepadBack);

            // left stick
            HandleGamepadStickEvent(
                io,
                GamepadAxis.LeftX,
                ImGuiKey.GamepadLStickLeft,
                ImGuiKey.GamepadLStickRight
            );
            HandleGamepadStickEvent(
                io,
                GamepadAxis.LeftY,
                ImGuiKey.GamepadLStickUp,
                ImGuiKey.GamepadLStickDown
            );

            // right stick
            HandleGamepadStickEvent(
                io,
                GamepadAxis.RightX,
                ImGuiKey.GamepadRStickLeft,
                ImGuiKey.GamepadRStickRight
            );
            HandleGamepadStickEvent(
                io,
                GamepadAxis.RightY,
                ImGuiKey.GamepadRStickUp,
                ImGuiKey.GamepadRStickDown
            );
        }

        return true;
    }
}
