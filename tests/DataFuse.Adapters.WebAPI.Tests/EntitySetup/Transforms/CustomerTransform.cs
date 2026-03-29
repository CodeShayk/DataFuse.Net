using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.ResultTransformers
{
    public class CustomerTransform : BaseTransformer<CustomerResult, Customer>
    {
        public override void Transform(CustomerResult result, Customer contract)
        {
            var customer = contract ?? new Customer();
            customer.Id = result.Id;
            customer.Name = result.Name;
            customer.Code = result.Code;

            if (result is WebHeaderResult webResult)
                if (webResult.Headers.TryGetValue("x-meta-branch-code", out var branch))
                    customer.Branch = branch;
        }
    }
}