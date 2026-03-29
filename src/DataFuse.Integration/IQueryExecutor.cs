using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration;

public interface IQueryExecutor
{
    Task<IList<IQueryResult>> ExecuteAsync(IDataContext context, IQueryList queries);
}
