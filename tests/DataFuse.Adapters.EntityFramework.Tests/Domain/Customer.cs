namespace DataFuse.Adapters.EntityFramework.Tests.Domain
{
    public class Customer
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public Communication Communication { get; set; }
        public Order[] Orders { get; set; }
    }
}