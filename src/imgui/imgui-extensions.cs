using System.Numerics;
using Hexa.NET.ImGui;

namespace Black.DuskPicker;

public static class ImGuiStyleX
{
    extension(ImGui)
    {
        public static void UseFont(ImFontPtr font, Action callback)
        {
            ImGui.PushFont(font);
            callback();
            ImGui.PopFont();
        }

        public static unsafe void SetNextWindowSizeConstraints<T>(Vector2 sizeMin, Vector2 sizeMax, ImGuiSizeCallback customCallback, T userData)
            where T : unmanaged
        {
            ImGui.SetNextWindowSizeConstraints(sizeMin, sizeMax, customCallback, (void*)&userData);
        }
    }
    
    extension(ImGuiStylePtr style)
    {
        
        public Vector4 TextColor 
        { 
            get => style.Colors[(int)ImGuiCol.Text]; 
            set => style.Colors[(int)ImGuiCol.Text] = value; 
        }
        
        public Vector4 TextDisabledColor 
        { 
            get => style.Colors[(int)ImGuiCol.TextDisabled]; 
            set => style.Colors[(int)ImGuiCol.TextDisabled] = value; 
        }
        
        public Vector4 WindowBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.WindowBg]; 
            set => style.Colors[(int)ImGuiCol.WindowBg] = value; 
        }
        
        public Vector4 ChildBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.ChildBg]; 
            set => style.Colors[(int)ImGuiCol.ChildBg] = value; 
        }
        
        public Vector4 PopupBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.PopupBg]; 
            set => style.Colors[(int)ImGuiCol.PopupBg] = value; 
        }
        
        public Vector4 BorderColor 
        { 
            get => style.Colors[(int)ImGuiCol.Border]; 
            set => style.Colors[(int)ImGuiCol.Border] = value; 
        }
        
        public Vector4 BorderShadowColor 
        { 
            get => style.Colors[(int)ImGuiCol.BorderShadow]; 
            set => style.Colors[(int)ImGuiCol.BorderShadow] = value; 
        }
        
        public Vector4 FrameBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.FrameBg]; 
            set => style.Colors[(int)ImGuiCol.FrameBg] = value; 
        }
        
        public Vector4 FrameBgHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.FrameBgHovered]; 
            set => style.Colors[(int)ImGuiCol.FrameBgHovered] = value; 
        }
        
        public Vector4 FrameBgActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.FrameBgActive]; 
            set => style.Colors[(int)ImGuiCol.FrameBgActive] = value; 
        }
        
        public Vector4 TitleBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.TitleBg]; 
            set => style.Colors[(int)ImGuiCol.TitleBg] = value; 
        }
        
        public Vector4 TitleBgActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.TitleBgActive]; 
            set => style.Colors[(int)ImGuiCol.TitleBgActive] = value; 
        }
        
        public Vector4 TitleBgCollapsedColor 
        { 
            get => style.Colors[(int)ImGuiCol.TitleBgCollapsed]; 
            set => style.Colors[(int)ImGuiCol.TitleBgCollapsed] = value; 
        }
        
        public Vector4 MenuBarBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.MenuBarBg]; 
            set => style.Colors[(int)ImGuiCol.MenuBarBg] = value; 
        }
        
        public Vector4 ScrollbarBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.ScrollbarBg]; 
            set => style.Colors[(int)ImGuiCol.ScrollbarBg] = value; 
        }
        
        public Vector4 ScrollbarGrabColor 
        { 
            get => style.Colors[(int)ImGuiCol.ScrollbarGrab]; 
            set => style.Colors[(int)ImGuiCol.ScrollbarGrab] = value; 
        }
        
        public Vector4 ScrollbarGrabHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]; 
            set => style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = value; 
        }
        
        public Vector4 ScrollbarGrabActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.ScrollbarGrabActive]; 
            set => style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = value; 
        }
        
        public Vector4 CheckMarkColor 
        { 
            get => style.Colors[(int)ImGuiCol.CheckMark]; 
            set => style.Colors[(int)ImGuiCol.CheckMark] = value; 
        }
        
        public Vector4 SliderGrabColor 
        { 
            get => style.Colors[(int)ImGuiCol.SliderGrab]; 
            set => style.Colors[(int)ImGuiCol.SliderGrab] = value; 
        }
        
        public Vector4 SliderGrabActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.SliderGrabActive]; 
            set => style.Colors[(int)ImGuiCol.SliderGrabActive] = value; 
        }
        
        public Vector4 ButtonColor 
        { 
            get => style.Colors[(int)ImGuiCol.Button]; 
            set => style.Colors[(int)ImGuiCol.Button] = value; 
        }
        
        public Vector4 ButtonHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.ButtonHovered]; 
            set => style.Colors[(int)ImGuiCol.ButtonHovered] = value; 
        }
        
        public Vector4 ButtonActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.ButtonActive]; 
            set => style.Colors[(int)ImGuiCol.ButtonActive] = value; 
        }
        
        public Vector4 HeaderColor 
        { 
            get => style.Colors[(int)ImGuiCol.Header]; 
            set => style.Colors[(int)ImGuiCol.Header] = value; 
        }
        
        public Vector4 HeaderHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.HeaderHovered]; 
            set => style.Colors[(int)ImGuiCol.HeaderHovered] = value; 
        }
        
        public Vector4 HeaderActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.HeaderActive]; 
            set => style.Colors[(int)ImGuiCol.HeaderActive] = value; 
        }
        
        public Vector4 SeparatorColor 
        { 
            get => style.Colors[(int)ImGuiCol.Separator]; 
            set => style.Colors[(int)ImGuiCol.Separator] = value; 
        }
        
        public Vector4 SeparatorHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.SeparatorHovered]; 
            set => style.Colors[(int)ImGuiCol.SeparatorHovered] = value; 
        }
        
        public Vector4 SeparatorActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.SeparatorActive]; 
            set => style.Colors[(int)ImGuiCol.SeparatorActive] = value; 
        }
        
        public Vector4 ResizeGripColor 
        { 
            get => style.Colors[(int)ImGuiCol.ResizeGrip]; 
            set => style.Colors[(int)ImGuiCol.ResizeGrip] = value; 
        }
        
        public Vector4 ResizeGripHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.ResizeGripHovered]; 
            set => style.Colors[(int)ImGuiCol.ResizeGripHovered] = value; 
        }
        
        public Vector4 ResizeGripActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.ResizeGripActive]; 
            set => style.Colors[(int)ImGuiCol.ResizeGripActive] = value; 
        }
        
        public Vector4 TabColor 
        { 
            get => style.Colors[(int)ImGuiCol.Tab]; 
            set => style.Colors[(int)ImGuiCol.Tab] = value; 
        }
        
        public Vector4 TabHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.TabHovered]; 
            set => style.Colors[(int)ImGuiCol.TabHovered] = value; 
        }
        
        public Vector4 TabActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.TabSelected]; 
            set => style.Colors[(int)ImGuiCol.TabSelected] = value; 
        }
        
        public Vector4 TabUnfocusedColor 
        { 
            get => style.Colors[(int)ImGuiCol.TabDimmed]; 
            set => style.Colors[(int)ImGuiCol.TabDimmed] = value; 
        }
        
        public Vector4 TabUnfocusedActiveColor 
        { 
            get => style.Colors[(int)ImGuiCol.TabDimmedSelected]; 
            set => style.Colors[(int)ImGuiCol.TabDimmedSelected] = value; 
        }
        
        public Vector4 PlotLinesColor 
        { 
            get => style.Colors[(int)ImGuiCol.PlotLines]; 
            set => style.Colors[(int)ImGuiCol.PlotLines] = value; 
        }
        
        public Vector4 PlotLinesHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.PlotLinesHovered]; 
            set => style.Colors[(int)ImGuiCol.PlotLinesHovered] = value; 
        }
        
        public Vector4 PlotHistogramColor 
        { 
            get => style.Colors[(int)ImGuiCol.PlotHistogram]; 
            set => style.Colors[(int)ImGuiCol.PlotHistogram] = value; 
        }
        
        public Vector4 PlotHistogramHoveredColor 
        { 
            get => style.Colors[(int)ImGuiCol.PlotHistogramHovered]; 
            set => style.Colors[(int)ImGuiCol.PlotHistogramHovered] = value; 
        }
        
        public Vector4 TableHeaderBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.TableHeaderBg]; 
            set => style.Colors[(int)ImGuiCol.TableHeaderBg] = value; 
        }
        
        public Vector4 TableBorderStrongColor 
        { 
            get => style.Colors[(int)ImGuiCol.TableBorderStrong]; 
            set => style.Colors[(int)ImGuiCol.TableBorderStrong] = value; 
        }
        
        public Vector4 TableBorderLightColor 
        { 
            get => style.Colors[(int)ImGuiCol.TableBorderLight]; 
            set => style.Colors[(int)ImGuiCol.TableBorderLight] = value; 
        }
        
        public Vector4 TableRowBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.TableRowBg]; 
            set => style.Colors[(int)ImGuiCol.TableRowBg] = value; 
        }
        
        public Vector4 TableRowBgAltColor 
        { 
            get => style.Colors[(int)ImGuiCol.TableRowBgAlt]; 
            set => style.Colors[(int)ImGuiCol.TableRowBgAlt] = value; 
        }
        
        public Vector4 TextSelectedBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.TextSelectedBg]; 
            set => style.Colors[(int)ImGuiCol.TextSelectedBg] = value; 
        }
        
        public Vector4 DragDropTargetColor 
        { 
            get => style.Colors[(int)ImGuiCol.DragDropTarget]; 
            set => style.Colors[(int)ImGuiCol.DragDropTarget] = value; 
        }
        
        public Vector4 NavHighlightColor 
        { 
            get => style.Colors[(int)ImGuiCol.NavCursor]; 
            set => style.Colors[(int)ImGuiCol.NavCursor] = value; 
        }
        
        public Vector4 NavWindowingHighlightColor 
        { 
            get => style.Colors[(int)ImGuiCol.NavWindowingHighlight]; 
            set => style.Colors[(int)ImGuiCol.NavWindowingHighlight] = value; 
        }
        
        public Vector4 NavWindowingDimBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.NavWindowingDimBg]; 
            set => style.Colors[(int)ImGuiCol.NavWindowingDimBg] = value; 
        }
        
        public Vector4 ModalWindowDimBgColor 
        { 
            get => style.Colors[(int)ImGuiCol.ModalWindowDimBg]; 
            set => style.Colors[(int)ImGuiCol.ModalWindowDimBg] = value; 
        }
    }
}
