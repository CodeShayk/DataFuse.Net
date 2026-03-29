using Microsoft.Extensions.DependencyInjection;
using DataFuse.Integration;
using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.Entities;

namespace DataFuse.Adapters.EntityFramework.Tests;

[TestFixture]
public class E2ETests : BaseTest
{
    private IDataProvider<Customer> _provider;

    [SetUp]
    public void Setup()
    {
        _provider = _serviceProvider.GetService<IDataProvider<Customer>>();
    }

    [Test]
    public async Task TestDataProviderToFetchWholeEntityWhenPathsAreNull()
    {
        var customer = await _provider.GetDataAsync(new CustomerRequest
        {
            CustomerId = 1
        });

        var expected = new Customer
        {
            Id = 1,
            Name = "Jack Sparrow",
            Code = "AB123",
            Communication = new Communication
            {
                ContactId = 1,
                Phone = "0123456789",
                Email = "jack.sparrow@gmail.com",
                Address = new Address
                {
                    AddressId = 1,
                    HouseNo = "77",
                    City = "Wansted",
                    Region = "Belfast",
                    PostalCode = "BL34Y56",
                    Country = "United Kingdom",
                }
            },
            Orders = [ new Order {
                    OrderId = 1,
                    OrderNo = "ZX123VH",
                    Date = DateTime.Parse("2021-10-22T12:13:04"),
                    Items =
                    [
                        new OrderItem
                        {
                            ItemId = 1, Name = "12 inch Cake", Cost = 30m
                        },
                        new OrderItem
                        {
                            ItemId = 2, Name = "20 Cake Candles", Cost = 5m
                        }
                    ]
                }]
        };

        AssertAreEqual(expected, customer);
    }

    [Test]
    public async Task TestDataProviderToFetchEntityWhenPathsContainsCommunication()
    {
        var customer = await _provider.GetDataAsync(new CustomerRequest
        {
            CustomerId = 1,
            SchemaPaths = new[] { "Customer/Communication" }
        });

        var expected = new Customer
        {
            Id = 1,
            Name = "Jack Sparrow",
            Code = "AB123",
            Communication = new Communication
            {
                ContactId = 1,
                Phone = "0123456789",
                Email = "jack.sparrow@gmail.com",
                Address = new Address
                {
                    AddressId = 1,
                    HouseNo = "77",
                    City = "Wansted",
                    Region = "Belfast",
                    PostalCode = "BL34Y56",
                    Country = "United Kingdom",
                }
            }
        };

        AssertAreEqual(expected, customer);
    }

    [Test]
    public async Task TestDataProviderToFetchEntityWhenPathsContainsOrderItems()
    {
        var customer = await _provider.GetDataAsync(new CustomerRequest
        {
            CustomerId = 1,
            SchemaPaths = new[] { "Customer/orders/order/items/item" }
        });

        var expected = new Customer
        {
            Id = 1,
            Name = "Jack Sparrow",
            Code = "AB123",
            Orders = [ new Order
                {
                    OrderId = 1,
                    OrderNo = "ZX123VH",
                    Date = DateTime.Parse("2021-10-22T12:13:04"),
                    Items =
                    [
                        new OrderItem
                        {
                            ItemId = 1, Name = "12 inch Cake", Cost = 30m
                        },
                        new OrderItem
                        {
                            ItemId = 2, Name = "20 Cake Candles", Cost = 5m
                        }
                    ]
                }]
        };

        AssertAreEqual(expected, customer);
    }
}
