namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.Entities;

public class Order
{
    public int OrderId { get; set; }
    public string? OrderNo { get; set; }
    public DateTime Date { get; set; }
    public OrderItem[]? Items { get; set; }
}
