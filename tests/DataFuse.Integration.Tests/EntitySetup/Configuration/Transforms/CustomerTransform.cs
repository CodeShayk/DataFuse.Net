using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;
using DataFuse.Integration.Tests.EntitySetup.Entities;

namespace DataFuse.Integration.Tests.EntitySetup.Configuration.Transforms
{
    public class CustomerTransform : BaseTransformer<CustomerRecord, Customer>
    {
        public override void Transform(CustomerRecord queryResult, Customer entity)
        {
            var customer = entity ?? new Customer();
            customer.Id = queryResult.Id;
            customer.Name = queryResult.CustomerName;
            customer.Code = queryResult.CustomerCode;
        }
    }
}