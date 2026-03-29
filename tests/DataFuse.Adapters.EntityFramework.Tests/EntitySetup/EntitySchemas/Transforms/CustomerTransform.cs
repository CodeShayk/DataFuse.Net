using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.Entities;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Transforms
{
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
}