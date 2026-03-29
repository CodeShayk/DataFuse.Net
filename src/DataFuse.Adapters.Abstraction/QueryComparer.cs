namespace DataFuse.Adapters.Abstraction;

public class QueryComparer : IEqualityComparer<IQuery>
{
    public bool Equals(IQuery? x, IQuery? y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return x.GetType() == y.GetType();
    }

    public int GetHashCode(IQuery obj)
    {
        Guard.ThrowIfNull(obj, nameof(obj));
        return obj.GetType().GetHashCode();
    }
}
