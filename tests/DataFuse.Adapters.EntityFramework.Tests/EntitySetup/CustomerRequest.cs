using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup
{
    internal class CustomerRequest : IEntityRequest
    {
        public int CustomerId { get; set; }
        public string[] SchemaPaths { get; set; }
    }
}