using global::MongoDB.Bson;
using global::MongoDB.Driver;
using Testcontainers.MongoDb;

namespace DataFuse.Adapters.MongoDB.Tests.Integration;

[SetUpFixture]
public class MongoContainerFixture
{
    public static MongoDbContainer Container { get; private set; } = null!;
    public static IMongoDatabase Database { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        Container = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .Build();

        await Container.StartAsync();

        var client = new MongoClient(Container.GetConnectionString());
        Database = client.GetDatabase("datafuse_test");

        await SeedData();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        await Container.DisposeAsync();
    }

    private async Task SeedData()
    {
        var customers = Database.GetCollection<BsonDocument>("customers");
        await customers.InsertOneAsync(new BsonDocument
        {
            { "customerId", 1 },
            { "code", "AB123" },
            { "name", "Jack Sparrow" }
        });

        var communications = Database.GetCollection<BsonDocument>("communications");
        await communications.InsertOneAsync(new BsonDocument
        {
            { "contactId", 1 },
            { "phone", "0123456789" },
            { "email", "jack.sparrow@gmail.com" },
            { "addressId", 1 },
            { "houseNo", "77" },
            { "city", "Wansted" },
            { "region", "Belfast" },
            { "postalCode", "BL34Y56" },
            { "country", "United Kingdom" }
        });

        var orders = Database.GetCollection<BsonDocument>("orders");
        await orders.InsertOneAsync(new BsonDocument
        {
            { "orderId", 1 },
            { "customerId", 1 },
            { "orderNo", "ZX123VH" },
            { "orderDate", new DateTime(2021, 10, 22, 12, 13, 4, DateTimeKind.Utc) }
        });

        var orderItems = Database.GetCollection<BsonDocument>("orderItems");
        await orderItems.InsertManyAsync(new[]
        {
            new BsonDocument
            {
                { "orderId", 1 },
                { "itemId", 1 },
                { "name", "12 inch Cake" },
                { "cost", 30m }
            },
            new BsonDocument
            {
                { "orderId", 1 },
                { "itemId", 2 },
                { "name", "20 Cake Candles" },
                { "cost", 5m }
            }
        });
    }
}
