using DataFuse.Adapters.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

[BsonNoId]
[BsonIgnoreExtraElements]
public class OrderRecord : IQueryResult
{
    [BsonElement("orderId")]
    public int OrderId { get; set; }

    [BsonElement("orderNo")]
    public string? OrderNo { get; set; }

    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; }
}
