namespace DataFuse.Adapters.SQL.Tests.EntitySetup.Entities
{
    public class Communication
    {
        public int ContactId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }
}