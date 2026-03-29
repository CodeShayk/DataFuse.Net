using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.Entities;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Transforms;

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
