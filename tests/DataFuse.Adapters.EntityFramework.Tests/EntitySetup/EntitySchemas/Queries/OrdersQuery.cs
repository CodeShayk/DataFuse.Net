using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.Domain;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class OrdersQuery : SQLQuery<CollectionResult<OrderRecord>>
    {
        protected override Func<DbContext, Task<CollectionResult<OrderRecord>>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to customer query.
            var customer = (CustomerRecord)parentQueryResult;

            return async dbContext =>
            {
                var items = await dbContext.Set<Order>()
                .Where(p => p.Customer.Id == customer.Id)
                .Select(c => new OrderRecord
                {
                    CustomerId = c.CustomerId,
                    OrderId = c.OrderId,
                    Date = c.Date,
                    OrderNo = c.OrderNo
                })
                .ToListAsync();

                return new CollectionResult<OrderRecord>(items);
            };
        }
    }
}