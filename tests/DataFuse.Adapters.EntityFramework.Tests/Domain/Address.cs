namespace DataFuse.Adapters.EntityFramework.Tests.Domain
{
    public class Address
    {
        public int AddressId { get; set; }
        public int CommunicationId { get; set; }

        public Communication Communication { get; set; }

        public string HouseNo { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
    }
}