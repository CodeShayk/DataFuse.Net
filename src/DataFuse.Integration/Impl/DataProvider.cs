using System.Diagnostics;
using Microsoft.Extensions.Logging;
using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.PathMatchers;

namespace DataFuse.Integration.Impl;

public class DataProvider<TEntity> : IDataProvider<TEntity>
    where TEntity : IEntity, new()
{
    private readonly ILogger<IDataProvider<TEntity>>? logger;
    private readonly IQueryExecutor queryExecutor;
    private readonly IQueryBuilder<TEntity> queryBuilder;
    private readonly IEntityBuilder<TEntity> entityBuilder;

    public DataProvider(
        IEntityConfiguration<TEntity> entitySchema,
        params IQueryEngine[] queryEngines)
        : this(null, new QueryBuilder<TEntity>(entitySchema, new XPathMatcher()),
          new QueryExecutor(queryEngines), new EntityBuilder<TEntity>(entitySchema))
    {
    }

    public DataProvider(
        ILogger<IDataProvider<TEntity>>? logger,
        IEntityConfiguration<TEntity> entitySchema,
        ISchemaPathMatcher schemaPathMatcher,
        params IQueryEngine[] queryEngines)
        : this(logger, new QueryBuilder<TEntity>(entitySchema, schemaPathMatcher),
          new QueryExecutor(queryEngines), new EntityBuilder<TEntity>(entitySchema))
    {
    }

    public DataProvider(
        ILogger<IDataProvider<TEntity>>? logger,
        IQueryBuilder<TEntity> queryBuilder,
        IQueryExecutor queryExecutor,
        IEntityBuilder<TEntity> entityBuilder)
    {
        this.logger = logger;
        this.queryBuilder = queryBuilder;
        this.queryExecutor = queryExecutor;
        this.entityBuilder = entityBuilder;
    }

    public Task<TEntity> GetDataAsync(IEntityRequest request)
    {
        var context = new DataContext(request);
        return GetDataAsync(context);
    }

    internal async Task<TEntity> GetDataAsync(IDataContext context)
    {
        // Build queries for the data source based on the included xPaths
        var watch = Stopwatch.StartNew();
        var queries = queryBuilder.Build(context);

        watch.Stop();
        logger?.LogInformation("Query builder executed in {ElapsedMs} ms", watch.ElapsedMilliseconds);

        // execute all queries to get results
        watch = Stopwatch.StartNew();
        var results = await queryExecutor.ExecuteAsync(context, queries);
        watch.Stop();
        logger?.LogInformation("Query executor executed in {ElapsedMs} ms", watch.ElapsedMilliseconds);

        // Executes configured transformers to map query results to target entity
        watch = Stopwatch.StartNew();
        var entity = entityBuilder.Build(context, results);
        watch.Stop();
        logger?.LogInformation("Transform executor executed in {ElapsedMs} ms", watch.ElapsedMilliseconds);

        return entity;
    }
}
