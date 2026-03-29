using System.Data;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL;

public abstract class SQLQuery<TQueryResult> : BaseQuery<TQueryResult>, ISQLQuery
       where TQueryResult : IQueryResult
{
    async Task<IQueryResult> ISQLQuery.Run(IDbConnection conn)
    {
        return await QueryDelegate!(conn);
    }

    private Func<IDbConnection, Task<TQueryResult>>? QueryDelegate;

    public override bool IsContextResolved() => QueryDelegate is not null;

    public override void ResolveQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        QueryDelegate = GetQuery(context, parentQueryResult);
    }

    /// <summary>
    /// Get query delegate to return query result.
    /// </summary>
    protected abstract Func<IDbConnection, Task<TQueryResult>> GetQuery(IDataContext context, IQueryResult? parentQueryResult);
}
