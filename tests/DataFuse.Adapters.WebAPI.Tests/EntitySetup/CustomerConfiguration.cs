using DataFuse.Adapters.WebAPI.Tests.EntitySetup.ResultTransformers;
using DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup
{
    internal class CustomerConfiguration : EntityConfiguration<Customer>
    {
        /// <summary>
        /// Constructs the api aggregate with web apis and result transformers to map data to aggregated contract.
        /// </summary>
        /// <returns>Mappings</returns>
        public override IEnumerable<Mapping<Customer, IQueryResult>> GetSchema()
        {
            return CreateSchema.For<Customer>()
                .Map<CustomerWebQuery, CustomerTransform>(For.Paths("customer"),
                 customer => customer.Dependents
                    .Map<CommunicationWebQuery, CommunicationTransform>(For.Paths("customer.communication"))
                    .Map<OrdersWebQuery, OrdersTransform>(For.Paths("customer.orders"),
                        customerOrders => customerOrders.Dependents
                            .Map<OrderItemsWebQuery, OrderItemsTransform>(For.Paths("customer.orders.items")))
                ).End();
        }
    }
}