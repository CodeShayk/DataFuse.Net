namespace DataFuse.Adapters.Abstraction;

public interface IEntityContextCache
{
    /// <summary>
    /// Cache dictionary holding query results for query result type marked with [CacheResult] attribute.
    /// </summary>
    Dictionary<string, object> Cache { get; set; }
}
