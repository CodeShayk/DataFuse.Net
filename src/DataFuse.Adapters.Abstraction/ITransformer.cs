namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Implement transformer to map data from supported query result to entity in context.
/// </summary>
public interface ITransformer
{
    /// <summary>
    /// Transform method to map data to entity for a given query result.
    /// </summary>
    void Transform(IQueryResult queryResult, IEntity entity);
}
