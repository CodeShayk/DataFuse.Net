using System.Data;
using Dapper;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class OrdersQuery : SQLQuery<CollectionResult<OrderRecord>>
    {
        protected override Func<IDbConnection, Task<CollectionResult<OrderRecord>>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to customer query.
            var customer = (CustomerRecord)parentQueryResult;

            return async connection =>
            {
                var items = await connection.QueryAsync<OrderRecord>(new CommandDefinition
                (
                    "select OrderId, " +
                            "OrderNo, " +
                            "OrderDate " +
                        "from TOrder " +
                    $"where customerId={customer.Id}"
                ));

                return new CollectionResult<OrderRecord>(items);
            };
        }
    }
}