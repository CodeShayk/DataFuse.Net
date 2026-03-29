namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
    public class OrderItemRecord
    {
        public int OrderId { get; set; }
        public (int ItemId, string Name, decimal Cost)[] Items { get; set; }
    }
}