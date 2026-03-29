using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using DataFuse.Adapters.SQL.Tests.EntitySetup.Entities;
using DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Transforms
{
    public class OrderItemsTransform : BaseTransformer<CollectionResult<OrderItemRecord>, Customer>
    {
        public override void Transform(CollectionResult<OrderItemRecord> collectionResult, Customer customer)
        {
            if (collectionResult == null || !collectionResult.Any() || customer.Orders == null)
                return;

            foreach (var result in collectionResult)
            {
                var order = customer.Orders.FirstOrDefault(o => o.OrderId == result.OrderId);
                if (order == null)
                    continue;

                order.Items = ArrayUtil.EnsureAndResizeArray(order.Items, out var index);
                order.Items[index] = new OrderItem
                {
                    ItemId = result.ItemId,
                    Name = result.Name,
                    Cost = result.Cost
                };
            }
        }
    }
}