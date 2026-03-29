namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Implement to get supported Query result.
/// </summary>
public interface ITransformerQueryResult
{
    /// <summary>
    /// Supported query result.
    /// </summary>
    Type SupportedQueryResult { get; }
}
