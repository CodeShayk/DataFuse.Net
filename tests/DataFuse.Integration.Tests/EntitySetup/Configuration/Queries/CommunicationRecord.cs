namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Queries
{
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
    }
}