using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup
{
    public class Customer : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public Contacts Communication { get; set; }
        public Order[] Orders { get; set; }

        public class Contacts
        {
            public int ContactId { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public Address PostalAddress { get; set; }

            public class Address
            {
                public string HouseNo { get; set; }
                public string City { get; set; }
                public string Region { get; set; }
                public string PostalCode { get; set; }
                public string Country { get; set; }
            }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public string OrderNo { get; set; }
            public DateTime Date { get; set; }
            public OrderItem[] Items { get; set; }

            public class OrderItem
            {
                public int ItemId { get; set; }
                public string Name { get; set; }
                public decimal Cost { get; set; }
            }
        }
    }
}