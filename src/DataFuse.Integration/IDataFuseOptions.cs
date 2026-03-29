using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration;

/// <summary>
/// Interface for DataFuse options builder.
/// </summary>
public interface IDataFuseOptions
{
    /// <summary>
    /// Register a query engine.
    /// </summary>
    IDataFuseOptions WithEngine(Func<IServiceProvider, IQueryEngine> queryEngines);

    /// <summary>
    /// Register a query engine of type <typeparamref name="TEngine"/>.
    /// </summary>
    IDataFuseOptions WithEngine<TEngine>() where TEngine : IQueryEngine;

    /// <summary>
    /// Register an array of query engines.
    /// </summary>
    IDataFuseOptions WithEngines(Func<IServiceProvider, IQueryEngine[]> queryEngines);

    /// <summary>
    /// Register an instance of ISchemaPathMatcher. Default is XPathMatcher.
    /// </summary>
    IDataFuseOptions WithPathMatcher(Func<IServiceProvider, ISchemaPathMatcher> pathMatcher);

    /// <summary>
    /// Register an instance of EntityConfiguration for the given entity type.
    /// </summary>
    IDataFuseOptions WithEntityConfiguration<TEntity>(Func<IServiceProvider, IEntityConfiguration<TEntity>> entityConfiguration)
        where TEntity : class, IEntity, new();

    /// <summary>
    /// Set DataFuse to silent mode. In this mode, no exceptions will be thrown if there are no entity configurations or query engines registered.
    /// </summary>
    IDataFuseOptions InSilentMode();
}
