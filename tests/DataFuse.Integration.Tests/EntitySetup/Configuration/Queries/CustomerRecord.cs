namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    public class CustomerRecord : IQueryResult
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }
}