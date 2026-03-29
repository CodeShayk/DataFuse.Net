using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.Domain;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class OrderItemsQuery : SQLQuery<CollectionResult<OrderItemRecord>>
    {
        protected override Func<DbContext, Task<CollectionResult<OrderItemRecord>>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to order query.
            var ordersResults = (CollectionResult<OrderRecord>)parentQueryResult;

            return async dbContext =>
            {
                var items = await dbContext.Set<OrderItem>()
               .Where(p => ordersResults.Select(o => o.OrderId).Contains(p.Order.OrderId))
               .Select(c => new OrderItemRecord
               {
                   ItemId = c.ItemId,
                   Name = c.Name,
                   Cost = c.Cost,
                   OrderId = c.Order.OrderId
               })
               .ToListAsync();

                return new CollectionResult<OrderItemRecord>(items);
            };
        }
    }
}