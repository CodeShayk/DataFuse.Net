namespace DataFuse.Integration.Helpers;

public static class Constraints
{
    public static void NotNull<T>(this T value)
    {
        Guard.ThrowIfNull(value, typeof(T).Name);
    }
}
