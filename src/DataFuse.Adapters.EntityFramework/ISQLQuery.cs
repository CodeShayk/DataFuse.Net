using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework;

public interface ISQLQuery
{
    /// <summary>
    /// Get query delegate with implementation to return query result.
    /// </summary>
    Task<IQueryResult> Run(DbContext dbContext);
}
