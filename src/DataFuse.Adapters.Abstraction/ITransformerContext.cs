namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Implement to set transform with data context.
/// </summary>
public interface ITransformerContext
{
    /// <summary>
    /// Implement to set context in transform.
    /// </summary>
    void SetContext(IDataContext context);
}
