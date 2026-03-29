using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB;

public interface IMongoQuery : IQuery
{
    Task<IQueryResult> Run(IMongoDatabase database);
}
