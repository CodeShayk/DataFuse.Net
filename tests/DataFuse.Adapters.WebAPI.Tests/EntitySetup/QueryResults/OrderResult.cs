using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults
{
    public class OrderResult : IQueryResult
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime Date { get; set; }
    }
}