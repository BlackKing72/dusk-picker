namespace Black.DuskPicker;

public abstract class Event
{
    public bool IsHandled { get; set; }
}

public sealed class WindowOpenEvent : Event { }

public sealed class WindowCloseEvent : Event { }

public static class EventSystem
{
    public static event Action<Event>? OnEvent;

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
