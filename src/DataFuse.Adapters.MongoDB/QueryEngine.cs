using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB;

public class QueryEngine : IQueryEngine
{
    private readonly IMongoDatabase _database;

    public QueryEngine(IMongoDatabase database)
    {
        Guard.ThrowIfNull(database, "MongoDB database instance is required.");
        _database = database;
    }

    public bool CanExecute(IQuery query) => query is IMongoQuery;

    public async Task<IQueryResult> Execute(IQuery query)
    {
        return await ((IMongoQuery)query).Run(_database);
    }
}
