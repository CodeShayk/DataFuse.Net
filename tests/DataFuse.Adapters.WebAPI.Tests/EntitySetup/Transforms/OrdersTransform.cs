using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;
using static DataFuse.Adapters.WebAPI.Tests.EntitySetup.Customer;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.ResultTransformers
{
    public class OrdersTransform : BaseTransformer<CollectionResult<OrderResult>, Customer>
    {
        public override void Transform(CollectionResult<OrderResult> collectionResult, Customer contract)
        {
            if (collectionResult == null || !collectionResult.Any())
                return;

            var customer = contract ?? new Customer();

            customer.Orders = new Order[collectionResult.Count];

            for (var index = 0; index < collectionResult.Count; index++)
                customer.Orders[index] = new Order
                {
                    Date = collectionResult[index].Date,
                    OrderId = collectionResult[index].OrderId,
                    OrderNo = collectionResult[index].OrderNo
                };
        }
    }
}