using Raylib_cs;

namespace Black.DuskPicker;

public abstract class Event
{
    public bool IsHandled { get; set; }
}

public sealed class WindowOpenEvent : Event { }

public sealed class WindowCloseEvent : Event { }

public sealed class WindowFocusEvent(bool focused) : Event
{
    public bool Focused { get; set; } = focused;
}

public class EventSystem
{
    public static event Action<Event>? OnEvent;

    private static readonly Observable<bool> isWindowFocused = new(false);
    private static readonly Observable<bool> isWindowHidden = new(false);

    static EventSystem()
    {
        isWindowFocused.Changed += static focused =>
        {
            RaiseEvent(new WindowFocusEvent(focused));
        };

        isWindowHidden.Changed += static hidden =>
        {
            if (hidden)
                RaiseEvent(new WindowCloseEvent());
            else
                RaiseEvent(new WindowOpenEvent());
        };

        isWindowFocused.SetWithoutNotify(Raylib.IsWindowFocused());
        isWindowHidden.SetWithoutNotify(Raylib.IsWindowHidden());
    }

    public static void OnUpdate()
    {
        isWindowFocused.Value = Raylib.IsWindowFocused();
        isWindowHidden.Value = Raylib.IsWindowHidden();
    }

    public static void RaiseEvent(Event evt)
    {
        OnEvent?.Invoke(evt);
    }
}

public struct EventDispatcher(Event data)
{
    public readonly bool Dispatch<T>(Action<T> dispatchHandler)
        where T : Event
    {
        if (data is T targetData)
            dispatchHandler(targetData);

        return false;
    }

    public readonly bool Dispatch<T>(Func<T, bool> dispatchHandler)
        where T : Event
    {
        if (data is T targetData)
        {
            targetData.IsHandled |= dispatchHandler(targetData);
            return true;
        }

        return false;
    }
}
