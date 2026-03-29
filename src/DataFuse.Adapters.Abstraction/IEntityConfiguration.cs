namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Implement to configure schema path mappings for an Entity.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IEntityConfiguration<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Entity schema mappings.
    /// </summary>
    IEnumerable<Mapping<TEntity, IQueryResult>> Mappings { get; }
}
