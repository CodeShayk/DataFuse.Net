namespace DataFuse.Integration.Helpers;

public static class StringExtensions
{
    public static bool IsNotNullOrEmpty(this string? value) => !string.IsNullOrEmpty(value);
}
