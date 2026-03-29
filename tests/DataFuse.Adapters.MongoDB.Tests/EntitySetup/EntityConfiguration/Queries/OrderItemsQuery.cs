using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

internal class OrderItemsQuery : MongoQuery<CollectionResult<OrderItemRecord>>
{
    protected override Func<IMongoDatabase, Task<CollectionResult<OrderItemRecord>>> GetQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        var ordersResult = (CollectionResult<OrderRecord>)parentQueryResult!;
        var orderIds = ordersResult.Select(o => o.OrderId).ToList();

        return async database =>
        {
            var collection = database.GetCollection<OrderItemRecord>("orderItems");
            var items = await collection.Find(i => orderIds.Contains(i.OrderId)).ToListAsync();
            return new CollectionResult<OrderItemRecord>(items);
        };
    }
}
