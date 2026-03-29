using System.Data;
using Dapper;
using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class OrderItemsQuery : SQLQuery<CollectionResult<OrderItemRecord>>
    {
        protected override Func<IDbConnection, Task<CollectionResult<OrderItemRecord>>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child query to order query taking OrderResult to resolve query parameter.
            var ordersResult = (CollectionResult<OrderRecord>)parentQueryResult;

            return async connection =>
            {
                var items = await connection.QueryAsync<OrderItemRecord>(new CommandDefinition
                   (
                       "select OrderId, " +
                              "OrderItemId as ItemId, " +
                              "Name, " +
                              "Cost " +
                       $"from TOrderItem where OrderId in ({ordersResult.Select(o => o.OrderId).ToCSV()})"
                  ));

                return new CollectionResult<OrderItemRecord>(items);
            };
        }
    }
}