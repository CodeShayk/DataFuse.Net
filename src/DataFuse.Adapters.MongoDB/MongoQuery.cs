using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB;

public abstract class MongoQuery<TQueryResult> : BaseQuery<TQueryResult>, IMongoQuery
    where TQueryResult : IQueryResult
{
    async Task<IQueryResult> IMongoQuery.Run(IMongoDatabase database)
    {
        return await QueryDelegate!(database);
    }

    private Func<IMongoDatabase, Task<TQueryResult>>? QueryDelegate;

    public override bool IsContextResolved() => QueryDelegate is not null;

    public override void ResolveQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        QueryDelegate = GetQuery(context, parentQueryResult);
    }

    /// <summary>
    /// Get query delegate to return query result from MongoDB.
    /// </summary>
    protected abstract Func<IMongoDatabase, Task<TQueryResult>> GetQuery(IDataContext context, IQueryResult? parentQueryResult);
}
