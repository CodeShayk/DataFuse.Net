using DataFuse.Adapters.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using DataFuse.Integration.Impl;

namespace DataFuse.Integration;

/// <summary>
/// Builder for DataFuse options.
/// </summary>
public class DataFuseOptionsBuilder : IDataFuseOptions
{
    public DataFuseOptionsBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
    public int EntityConfigurationCount { get; private set; }
    public int EngineCount { get; private set; }
    internal bool Silent { get; set; }

    IDataFuseOptions IDataFuseOptions.InSilentMode()
    {
        Silent = true;
        return this;
    }

    public IDataFuseOptions WithEngine(Func<IServiceProvider, IQueryEngine> queryEngine)
    {
        if (queryEngine is not null)
        {
            Services.AddScoped<IQueryEngine>(c => queryEngine(c));
            EngineCount++;
        }

        return this;
    }

    public IDataFuseOptions WithEngine<TEngine>() where TEngine : IQueryEngine
    {
        Services.AddScoped(typeof(IQueryEngine), typeof(TEngine));
        EngineCount++;

        return this;
    }

    public IDataFuseOptions WithEngines(Func<IServiceProvider, IQueryEngine[]> queryEngines)
    {
        if (queryEngines is not null)
        {
            Services.AddScoped<IQueryEngine[]>(c =>
            {
                var engines = queryEngines(c);
                EngineCount += engines.Length;
                return engines;
            });
        }

        return this;
    }

    /// <summary>
    /// Register an instance of ISchemaPathMatcher. Default is XPathMatcher.
    /// </summary>
    public IDataFuseOptions WithPathMatcher(Func<IServiceProvider, ISchemaPathMatcher> pathMatcher)
    {
        if (pathMatcher is not null)
        {
            Services.RemoveAll<ISchemaPathMatcher>();
            Services.AddScoped<ISchemaPathMatcher>(c => pathMatcher(c));
        }

        return this;
    }

    /// <summary>
    /// Register an instance of EntityConfiguration for the given entity type.
    /// You could register configuration for multiple entities.
    /// </summary>
    public IDataFuseOptions WithEntityConfiguration<TEntity>(Func<IServiceProvider, IEntityConfiguration<TEntity>> entityConfiguration)
        where TEntity : class, IEntity, new()
    {
        if (entityConfiguration is not null)
        {
            Services.AddTransient(typeof(IEntityConfiguration<TEntity>), c => entityConfiguration(c));

            Services.AddTransient<IQueryBuilder<TEntity>, QueryBuilder<TEntity>>(c => new QueryBuilder<TEntity>(c.GetService<IEntityConfiguration<TEntity>>()!, c.GetService<ISchemaPathMatcher>()!));
            Services.AddTransient<IEntityBuilder<TEntity>, EntityBuilder<TEntity>>(c => new EntityBuilder<TEntity>(c.GetService<IEntityConfiguration<TEntity>>()!));

            Services.AddTransient<IDataProvider<TEntity>, DataProvider<TEntity>>(
             c => new DataProvider<TEntity>(
                logger: c.GetService<ILogger<IDataProvider<TEntity>>>(),
                queryBuilder: c.GetService<IQueryBuilder<TEntity>>()!,
                queryExecutor: c.GetService<IQueryExecutor>()!,
                entityBuilder: c.GetService<IEntityBuilder<TEntity>>()!
            ));

            EntityConfigurationCount++;
        }

        return this;
    }
}
