using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.Entities;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Transforms
{
    public class OrdersTransform : BaseTransformer<CollectionResult<OrderRecord>, Customer>
    {
        public override void Transform(CollectionResult<OrderRecord> collectionResult, Customer entity)
        {
            if (collectionResult == null || !collectionResult.Any())
                return;

            var customer = entity ?? new Customer();

            customer.Orders = new Order[collectionResult.Count];

            for (var index = 0; index < collectionResult.Count; index++)
            {
                customer.Orders[index] = new Order
                {
                    Date = collectionResult[index].Date,
                    OrderId = collectionResult[index].OrderId,
                    OrderNo = collectionResult[index].OrderNo
                };
            }
        }
    }
}