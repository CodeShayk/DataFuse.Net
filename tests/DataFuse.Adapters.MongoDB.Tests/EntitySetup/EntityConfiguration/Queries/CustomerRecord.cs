using DataFuse.Adapters.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

[BsonNoId]
[BsonIgnoreExtraElements]
public class CustomerRecord : IQueryResult
{
    [BsonElement("customerId")]
    public int Id { get; set; }

    [BsonElement("code")]
    public string? Code { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }
}
