namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    internal class CommunicationQuery : BaseQuery<CommunicationRecord>
    {
        private object QueryParameter;

        public override bool IsContextResolved() => QueryParameter != null;

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to customer query.
            var customer = (CustomerRecord)parentQueryResult;
            QueryParameter = new
            {
                CustomerId = customer.Id
            };
        }
    }
}