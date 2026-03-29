namespace DataFuse.Adapters.MongoDB;

internal static class Guard
{
#if NET6_0_OR_GREATER
    internal static void ThrowIfNull(object? argument, string? paramName = null)
        => ArgumentNullException.ThrowIfNull(argument, paramName);
#else
    internal static void ThrowIfNull(object? argument, string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
    }
#endif
}
