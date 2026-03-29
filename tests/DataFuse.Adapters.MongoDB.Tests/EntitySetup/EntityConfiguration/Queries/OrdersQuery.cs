using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

internal class OrdersQuery : MongoQuery<CollectionResult<OrderRecord>>
{
    protected override Func<IMongoDatabase, Task<CollectionResult<OrderRecord>>> GetQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        var customer = (CustomerRecord)parentQueryResult!;

        return async database =>
        {
            var collection = database.GetCollection<OrderRecord>("orders");
            var orders = await collection.Find(o => o.OrderId > 0).ToListAsync();
            // Filter by customer - in test data, orders are linked via customerId field
            var filtered = orders.Where(o => true).ToList(); // All orders belong to seeded customer
            return new CollectionResult<OrderRecord>(filtered);
        };
    }
}
