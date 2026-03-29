using System.Data;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL;

public interface ISQLQuery : IQuery
{
    Task<IQueryResult> Run(IDbConnection conn);
}
