using System.Globalization;
using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.SQL.Tests.EntitySetup.Entities;
using DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Transforms
{
    public class OrdersTransform : BaseTransformer<CollectionResult<OrderRecord>, Customer>
    {
        public override void Transform(CollectionResult<OrderRecord> collectionResult, Customer contract)
        {
            if (collectionResult == null || !collectionResult.Any())
                return;

            var customer = contract ?? new Customer();

            customer.Orders = new Order[collectionResult.Count];

            for (var index = 0; index < collectionResult.Count; index++)
            {
                customer.Orders[index] = new Order
                {
                    Date = DateTime.ParseExact(collectionResult[index].OrderDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    OrderId = collectionResult[index].OrderId,
                    OrderNo = collectionResult[index].OrderNo
                };
            }
        }
    }
}