namespace DataFuse.Adapters.EntityFramework.Tests.Domain
{
    public class Communication
    {
        public int CommunicationId { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public Address Address { get; set; }
    }
}