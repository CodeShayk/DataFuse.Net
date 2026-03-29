using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup;
using MongoDB.Driver;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

public class CustomerQuery : MongoQuery<CustomerRecord>
{
    protected override Func<IMongoDatabase, Task<CustomerRecord>> GetQuery(IDataContext context, IQueryResult? parentQueryResult)
    {
        var customer = (CustomerRequest)context.Request;

        return async database =>
        {
            var collection = database.GetCollection<CustomerRecord>("customers");
            return await collection.Find(c => c.Id == customer.CustomerId).FirstOrDefaultAsync();
        };
    }
}
