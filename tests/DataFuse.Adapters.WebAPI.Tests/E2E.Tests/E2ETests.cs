using DataFuse.Integration.Helpers;
using NUnit.Framework;
using DataFuse.Adapters.WebAPI.Tests.EntitySetup;
using DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis;
using static DataFuse.Adapters.WebAPI.Tests.EntitySetup.Customer;
using static DataFuse.Adapters.WebAPI.Tests.EntitySetup.Customer.Contacts;

namespace DataFuse.Adapters.WebAPI.Tests.E2E.Tests;

[TestFixture]
public class E2ETests : BaseTest
{
    [SetUp]
    public void Setup()
    {
        StubApi(string.Format(Endpoints.BaseAddress + Endpoints.Customer, Endpoints.Ids.CustomerId), new { Id = 1000, Name = "John McKinsey", Code = "THG-UY6789" }, new Dictionary<string, string> { { "x-meta-branch-code", "London" } });
        StubApi(string.Format(Endpoints.BaseAddress + Endpoints.Communication, Endpoints.Ids.CustomerId), new { Id = 4567, Telephone = "07675998878", Email = "John.McKinsy@gmail.com", HouseNo = "22", City = "London", Region = "London", PostalCode = "W12 6GH", Country = "United Kingdom" });
        StubApi(string.Format(Endpoints.BaseAddress + Endpoints.Orders, Endpoints.Ids.CustomerId), new[] { new { OrderId = 1234, OrderNo = "GHK-897GB", Date = "2024-01-01T00:00:00" } });
        StubApi(string.Format(Endpoints.BaseAddress + Endpoints.OrderItems, Endpoints.Ids.CustomerId, Endpoints.Ids.OrderId), new[] { new { OrderId = 1234, ItemId = 2244, Name = "Pen", Cost = 12.00m }, new { OrderId = 1234, ItemId = 6677, Name = "Book", Cost = 15.00m } });
    }

    [TearDown]
    public void Teardown()
    {
    }

    [Test]
    public async Task TestDataProviderToFetchWholeContractWhenNamesAreNull()
    {
        var customer = await dataProvider.GetDataAsync(new CustomerRequest
        {
            CustomerId = Endpoints.Ids.CustomerId
        });

        var expected = new Customer
        {
            Id = 1000,
            Name = "John McKinsey",
            Code = "THG-UY6789",
            Branch = "London", // Received via response header
            Communication = new Contacts
            {
                ContactId = 4567,
                Phone = "07675998878",
                Email = "John.McKinsy@gmail.com",
                PostalAddress = new Address
                {
                    HouseNo = "22",
                    City = "London",
                    Region = "London",
                    PostalCode = "W12 6GH",
                    Country = "United Kingdom",
                }
            },
            Orders =
            [
                new Order
                {
                    OrderId = 1234,
                    OrderNo = "GHK-897GB",
                    Date = DateTime.Parse("2024-01-01T00:00:00"),
                    Items =
                    [
                        new Order.OrderItem
                        {
                            ItemId = 2244, Name = "Pen", Cost = 12.00m
                        },
                        new Order.OrderItem
                        {
                            ItemId = 6677, Name = "Book", Cost = 15.00m
                        }
                    ]
                }
            ]
        };

        AssertAreEqual(expected, customer);
    }

    [Test]
    public async Task TestDataProviderToFetchPartialCustomerOrdersContractWhenNamesAreIncluded()
    {
        var customer = await dataProvider.GetDataAsync(new CustomerRequest
        {
            CustomerId = Endpoints.Ids.CustomerId,
            SchemaPaths = ["customer.orders.items"]
        });

        var expected = new Customer
        {
            Id = 1000,
            Name = "John McKinsey",
            Code = "THG-UY6789",
            Branch = "London",
            Orders =
            [
                new Order
                {
                    OrderId = 1234,
                    OrderNo = "GHK-897GB",
                    Date = DateTime.Parse("2024-01-01T00:00:00"),
                    Items =
                    [
                        new Order.OrderItem
                        {
                            ItemId = 2244, Name = "Pen", Cost = 12.00m
                        },
                        new Order.OrderItem
                        {
                            ItemId = 6677, Name = "Book", Cost = 15.00m
                        }
                    ]
                }
            ]
        };

        AssertAreEqual(expected, customer);
    }

    [Test]
    public async Task TestDataProviderToFetchPartialCustomerCommunicationContractWhenNamesAreIncluded()
    {
        var customer = await dataProvider.GetDataAsync(new CustomerRequest
        {
            CustomerId = Endpoints.Ids.CustomerId,
            SchemaPaths = ["customer.communication"]
        });

        var expected = new Customer
        {
            Id = 1000,
            Name = "John McKinsey",
            Code = "THG-UY6789",
            Branch = "London",
            Communication = new Contacts
            {
                ContactId = 4567,
                Phone = "07675998878",
                Email = "John.McKinsy@gmail.com",
                PostalAddress = new Address
                {
                    HouseNo = "22",
                    City = "London",
                    Region = "London",
                    PostalCode = "W12 6GH",
                    Country = "United Kingdom",
                }
            }
        };

        AssertAreEqual(expected, customer);
    }

    private void AssertAreEqual(Customer expected, Customer actual)
    {
        var actualCustomer = actual.ToJson();
        var expectedCustomer = expected.ToJson();

        Console.WriteLine("expected:");
        Console.WriteLine(expectedCustomer);

        Console.WriteLine("actual:");
        Console.WriteLine(actualCustomer);

        Assert.That(actualCustomer, Is.EqualTo(expectedCustomer));
    }
}
