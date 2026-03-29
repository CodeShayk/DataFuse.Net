namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Implement to configure schema path mappings for an Entity.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity> where TEntity : IEntity
{
    public IEnumerable<Mapping<TEntity, IQueryResult>> Mappings { get; }

    protected EntityConfiguration()
    {
        Mappings = GetSchema();
    }

    /// <summary>
    /// Implement to configure entity schema mappings with queries & transformers.
    /// </summary>
    /// <returns>Entity Schema mappings</returns>
    public abstract IEnumerable<Mapping<TEntity, IQueryResult>> GetSchema();
}
