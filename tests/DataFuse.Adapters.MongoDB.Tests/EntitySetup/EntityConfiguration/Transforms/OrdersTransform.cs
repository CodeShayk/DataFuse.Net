using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.Entities;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Transforms;

public class OrdersTransform : BaseTransformer<CollectionResult<OrderRecord>, Customer>
{
    public override void Transform(CollectionResult<OrderRecord> collectionResult, Customer contract)
    {
        if (collectionResult == null || !collectionResult.Any())
            return;

        var customer = contract ?? new Customer();

        customer.Orders = new Order[collectionResult.Count];

        for (var index = 0; index < collectionResult.Count; index++)
        {
            customer.Orders[index] = new Order
            {
                Date = collectionResult[index].OrderDate,
                OrderId = collectionResult[index].OrderId,
                OrderNo = collectionResult[index].OrderNo
            };
        }
    }
}
