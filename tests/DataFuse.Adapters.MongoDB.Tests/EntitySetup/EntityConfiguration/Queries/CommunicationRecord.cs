using DataFuse.Adapters.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

[CacheResult]
[BsonNoId]
[BsonIgnoreExtraElements]
public class CommunicationRecord : IQueryResult
{
    [BsonElement("contactId")]
    public int ContactId { get; set; }

    [BsonElement("phone")]
    public string? Telephone { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("addressId")]
    public int AddressId { get; set; }

    [BsonElement("houseNo")]
    public string? HouseNo { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("region")]
    public string? Region { get; set; }

    [BsonElement("postalCode")]
    public string? PostalCode { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }
}
