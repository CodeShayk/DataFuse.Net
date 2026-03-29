namespace DataFuse.Adapters.EntityFramework.Tests.Domain
{
    public class OrderItem
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public Order Order { get; set; }
    }
}