namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    internal class OrdersQuery : BaseQuery<CollectionResult<OrderRecord>>
    {
        private object QueryParameter;

        public override bool IsContextResolved() => QueryParameter != null;

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Does not execute as child to any query.
            var customer = (CustomerRecord)parentQueryResult;
            QueryParameter = new
            {
                CustomerId = customer.Id
            };
        }
    }
}