using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.Entities;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Queries;

namespace DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration.Transforms;

public class CustomerTransform : BaseTransformer<CustomerRecord, Customer>
{
    public override void Transform(CustomerRecord queryResult, Customer entity)
    {
        var customer = entity ?? new Customer();
        customer.Id = queryResult.Id;
        customer.Name = queryResult.Name;
        customer.Code = queryResult.Code;
    }
}
