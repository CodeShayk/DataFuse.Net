using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.Entities
{
    public class Customer : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Communication Communication { get; set; }
        public Order[] Orders { get; set; }
    }
}