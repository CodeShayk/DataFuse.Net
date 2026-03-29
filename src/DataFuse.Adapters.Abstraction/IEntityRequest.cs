namespace DataFuse.Adapters.Abstraction;

public interface IEntityRequest
{
    /// <summary>
    /// Entity schema paths for data retrieval.
    /// </summary>
    string[] SchemaPaths { get; set; }
}
