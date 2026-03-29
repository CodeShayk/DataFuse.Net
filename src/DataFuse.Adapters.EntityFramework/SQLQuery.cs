using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework;

public abstract class SQLQuery<TQueryResult>
    : BaseQuery<TQueryResult>, ISQLQuery
   where TQueryResult : IQueryResult
{
    private Func<DbContext, Task<TQueryResult>>? QueryDelegate;

    public override bool IsContextResolved() => QueryDelegate is not null;

    public override void ResolveQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        QueryDelegate = GetQuery(context, parentQueryResult);
    }

    async Task<IQueryResult> ISQLQuery.Run(DbContext dbContext)
    {
        return await QueryDelegate!(dbContext);
    }

    /// <summary>
    /// Get query delegate to return query result.
    /// </summary>
    protected abstract Func<DbContext, Task<TQueryResult>> GetQuery(IDataContext context, IQueryResult? parentQueryResult);
}
