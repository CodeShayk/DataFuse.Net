using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

internal class CommunicationQuery : MongoQuery<CommunicationRecord>
{
    protected override Func<IMongoDatabase, Task<CommunicationRecord>> GetQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        var customer = (CustomerRecord)parentQueryResult!;

        return async database =>
        {
            var collection = database.GetCollection<CommunicationRecord>("communications");
            return await collection.Find(c => c.ContactId == customer.Id).FirstOrDefaultAsync();
        };
    }
}
