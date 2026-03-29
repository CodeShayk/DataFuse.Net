using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;
using DataFuse.Integration.Tests.EntitySetup.Entities;

namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Transforms
{
    public class OrdersTransform : BaseTransformer<CollectionResult<OrderRecord>, Customer>
    {
        public override void Transform(CollectionResult<OrderRecord> queryResult, Customer entity)
        {
            if (queryResult == null)
                return;

            var customer = entity ?? new Customer();
            customer.Orders = queryResult.Select(x => new Order
            {
                Date = x.Date,
                OrderId = x.OrderId,
                OrderNo = x.OrderNo
            }).ToArray();
        }
    }
}