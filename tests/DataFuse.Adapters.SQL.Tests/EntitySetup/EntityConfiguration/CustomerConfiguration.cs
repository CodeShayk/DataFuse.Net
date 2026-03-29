using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.SQL.Tests.EntitySetup.Entities;
using DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries;
using DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Transforms;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas
{
    internal class CustomerConfiguration : EntityConfiguration<Customer>
    {
        public override IEnumerable<Mapping<Customer, IQueryResult>> GetSchema()
        {
            return CreateSchema.For<Customer>()
                .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
                 customer => customer.Dependents
                    .Map<CommunicationQuery, CommunicationTransform>(For.Paths("customer/communication"))
                    .Map<OrdersQuery, OrdersTransform>(For.Paths("customer/orders"),
                        customerOrders => customerOrders.Dependents
                            .Map<OrderItemsQuery, OrderItemsTransform>(For.Paths("customer/orders/order/items")))
                ).End();
        }
    }
}