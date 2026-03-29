using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis
{
    internal class CommunicationWebQuery : WebQuery<CommunicationResult>
    {
        public CommunicationWebQuery() : base(Endpoints.BaseAddress)
        {
        }

        protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentApiResult)
        {
            var customer = (CustomerResult)parentApiResult;
            return () => new Uri(string.Format(Endpoints.BaseAddress + Endpoints.Communication, customer.Id), UriKind.Absolute);
        }
    }
}