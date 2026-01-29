namespace Black.DuskPicker;

public class Observable<T>(T initialValue)
{
    public event Action<T>? Changed;

    public T Value
    {
        get => field;
        set
        {
            var comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(field, value))
            {
                return;
            }

            field = value;
            Changed?.Invoke(value);
        }
    } = initialValue;

    public static implicit operator T(Observable<T> obs)
    {
        return obs.Value;
    }

    public static explicit operator Observable<T>(T value)
    {
        return new(value);
    }
}
