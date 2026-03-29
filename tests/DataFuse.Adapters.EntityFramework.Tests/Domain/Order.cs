namespace DataFuse.Adapters.EntityFramework.Tests.Domain
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        public string OrderNo { get; set; }
        public DateTime Date { get; set; }
        public Customer Customer { get; set; }
        public OrderItem[] Items { get; set; }
    }
}