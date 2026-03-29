namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    public class CustomerQuery : BaseQuery<CustomerRecord>
    {
        private object QueryParameter;

        public override bool IsContextResolved() => QueryParameter != null;

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Executes as root or level 1 query.
            var customer = (CustomerContext)context.Request;
            QueryParameter = new
            {
                customer.CustomerId
            };
        }
    }
}