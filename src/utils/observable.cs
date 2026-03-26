namespace Black.DuskPicker;

public class Observable<T>(T initialValue)
{
    public event Action<T>? Changed;

    private T value = initialValue;
    public T Value
    {
        get => value;
        set
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(this.value, value))
            {
                return;
            }

            this.value = value;
            Changed?.Invoke(value);
        }
    }

    public static implicit operator T(Observable<T> obs)
    {
        return obs.Value;
    }

    public static explicit operator Observable<T>(T value)
    {
        return new(value);
    }

    public void SetWithoutNotify(T value)
    {
        this.value = value;
    }
}
