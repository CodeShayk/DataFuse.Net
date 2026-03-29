using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using static DataFuse.Adapters.WebAPI.Tests.EntitySetup.Customer.Order;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.ResultTransformers
{
    public class OrderItemsTransform : BaseTransformer<CollectionResult<OrderItemResult>, Customer>
    {
        public override void Transform(CollectionResult<OrderItemResult> collectionResult, Customer customer)
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