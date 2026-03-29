using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis
{
    internal class OrdersWebQuery : WebQuery<CollectionResult<OrderResult>>
    {
        public OrdersWebQuery() : base(Endpoints.BaseAddress)
        {
        }

        protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentApiResult)
        {
            // Execute as child to customer api.
            var customer = (CustomerResult)parentApiResult;

            return () => new Uri(string.Format(Endpoints.BaseAddress + Endpoints.Orders, customer.Id), UriKind.Absolute);
        }
    }
}