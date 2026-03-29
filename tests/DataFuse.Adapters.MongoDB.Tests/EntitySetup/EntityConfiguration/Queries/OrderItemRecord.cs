using DataFuse.Adapters.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

[BsonNoId]
[BsonIgnoreExtraElements]
public class OrderItemRecord : IQueryResult
{
    [BsonElement("orderId")]
    public int OrderId { get; set; }

    [BsonElement("itemId")]
    public int ItemId { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("cost")]
    public decimal Cost { get; set; }
}
