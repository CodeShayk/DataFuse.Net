using System.Text;

namespace DataFuse.Integration.Helpers;

public static class EnumerableExtentions
{
    public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        foreach (var item in enumerable)
            action(item);
    }

    public static string? ToCSV<T>(this IEnumerable<T> instance, char separator)
    {
        if (instance is null)
            return null;

        if (!instance.Any())
            return string.Empty;

        var csv = new StringBuilder();
        instance.Each(value => csv.AppendFormat("{0}{1}", value, separator));
        return csv.ToString(0, csv.Length - 1);
    }

    public static string? ToCSV<T>(this IEnumerable<T> instance)
    {
        if (instance is null)
            return null;

        return instance.ToCSV(',');
    }
}
