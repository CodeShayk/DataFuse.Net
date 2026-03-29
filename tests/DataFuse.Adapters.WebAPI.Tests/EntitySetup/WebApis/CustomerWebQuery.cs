using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis
{
    public class CustomerWebQuery : WebQuery<CustomerResult>
    {
        public CustomerWebQuery() : base(Endpoints.BaseAddress)
        {
        }

        protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentApiResult)
        {
            // Executes as root or level 1 api.
            var customerContext = (CustomerRequest)context.Request;

            return () => new Uri(string.Format(Endpoints.BaseAddress + Endpoints.Customer, customerContext.CustomerId), UriKind.Absolute);
        }

        /// <summary>
        /// Override to pass outgoing request headers.
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> GetRequestHeaders()
        {
            return new Dictionary<string, string>
            {
                { "x-meta-branch-code", "London" }
            };
        }

        /// <summary>
        /// Override to subscribe for given Response headers to be added to Web query Result.
        /// For receiving response headers, You need to implement the TQueryResult type from WebResult class instead of IQueryResult.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetResponseHeaders()
        {
            return new[] { "x-meta-branch-code" };
        }
    }
}