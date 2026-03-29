using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework;

public class QueryEngine<T> : IQueryEngine where T : DbContext
{
    private readonly IDbContextFactory<T> _dbContextFactory;

    public QueryEngine(IDbContextFactory<T> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public bool CanExecute(IQuery query) => query is ISQLQuery;

    public async Task<IQueryResult> Execute(IQuery query)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await ((ISQLQuery)query).Run(dbContext);
    }
}
