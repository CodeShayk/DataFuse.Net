namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    internal class OrderItemsQuery : BaseQuery<CollectionResult<OrderItemRecord>>
    {
        private object QueryParameter;

        public override bool IsContextResolved() => QueryParameter != null;

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to order query.
            var ordersResult = (CollectionResult<OrderRecord>)parentQueryResult;
            QueryParameter = new
            {
                OrderIds = new List<int>(ordersResult.Select(x => x.OrderId))
            };
        }
    }
}