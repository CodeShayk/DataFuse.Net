using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;
using DataFuse.Integration.Tests.EntitySetup.Entities;

namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Transforms
{
    public class OrderItemsTransform : BaseTransformer<CollectionResult<OrderItemRecord>, Customer>
    {
        public override void Transform(CollectionResult<OrderItemRecord> queryResult, Customer entity)
        {
            if (queryResult == null || entity?.Orders == null)
                return;

            foreach (var item in queryResult.Where(x => x.Items != null))
                foreach (var order in entity.Orders)
                    if (order.OrderId == item.OrderId)
                        order.Items = item.Items.Select(x => new OrderItem
                        {
                            ItemId = x.ItemId,
                            Name = x.Name,
                            Cost = x.Cost
                        }).ToArray();
        }
    }
}