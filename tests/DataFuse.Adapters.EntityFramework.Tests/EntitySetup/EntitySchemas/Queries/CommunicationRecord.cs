using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries
{
    [CacheResult]
    public class CommunicationRecord : IQueryResult
    {
        public int Id { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string HouseNo { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int AddressId { get; internal set; }
    }
}