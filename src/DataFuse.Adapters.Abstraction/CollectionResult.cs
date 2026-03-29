namespace DataFuse.Adapters.Abstraction;

public class CollectionResult<T> : List<T>, IQueryResult
{
    public CollectionResult(IEnumerable<T> list) : base(list)
    {
    }

    public CollectionResult()
    {
    }
}
