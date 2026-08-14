using System.Numerics;
using Raylib_cs;

namespace Black.DuskPicker;

public static class RLMonitors
{
    public static Rectangle GetMonitorWorkArea(int monitorIndex)
    {
        return new(
            position: Raylib.GetMonitorPosition(monitorIndex),
            width: Raylib.GetMonitorWidth(monitorIndex),
            height: Raylib.GetMonitorHeight(monitorIndex));
    }

    public static Rectangle GetTotalWorkArea()
    {
        Vector2 position = new(float.MaxValue);
        Vector2 workArea = new(float.MinValue);

        int monitorCount = Raylib.GetMonitorCount();

        Raylib.TraceLog(TraceLogLevel.Debug, $"MONITORS: Founded {monitorCount} connected monitor(s):");

        for (int monitorIndex = 0; monitorIndex < monitorCount; monitorIndex++)
        {
            Rectangle monitor = GetMonitorWorkArea(monitorIndex);

            Raylib.TraceLog(TraceLogLevel.Debug, $"MONITORS:  Monitor {monitorIndex}: position: {monitor.Position} workArea: {monitor}");

            position = Vector2.Min(position, monitor.Position);
            workArea = Vector2.Max(workArea, monitor.Position + monitor.Size);
        }

        Raylib.TraceLog(TraceLogLevel.Debug, $"MONITORS:  Total work area: position: {position} workArea: {workArea}");

        return new Rectangle(position, workArea);
    }

    public static int GetMonitorUnderMouse(Vector2 mousePosition)
    {
        for (int monitorIndex = 0; monitorIndex < Raylib.GetMonitorCount(); monitorIndex++)
        {
            Rectangle monitorWorkArea = new(
                position: Raylib.GetMonitorPosition(monitorIndex),
                width: Raylib.GetMonitorWidth(monitorIndex),
                height: Raylib.GetMonitorHeight(monitorIndex));

            if (monitorWorkArea.IsInside(mousePosition))
                return monitorIndex;
        }

        return -1;
    }

    public static Vector2 LimitMouseToMonitor(Vector2 mousePosition, int monitorIndex = -1)
    {
        if (monitorIndex == -1)
            monitorIndex = GetMonitorUnderMouse(mousePosition);

        if (monitorIndex == -1)
            return mousePosition;

        Rectangle monitor = GetMonitorWorkArea(monitorIndex);
        if (monitor.IsInside(mousePosition))
            return mousePosition;

        Vector2 min = monitor.Position;
        Vector2 max = monitor.Position + monitor.Size;

        return new(
            Math.Clamp(mousePosition.X, min.X, max.X),
            Math.Clamp(mousePosition.Y, min.Y, max.Y));
    }
}
