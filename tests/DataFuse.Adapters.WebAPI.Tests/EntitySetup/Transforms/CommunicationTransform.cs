using DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.ResultTransformers
{
    public class CommunicationTransform : BaseTransformer<CommunicationResult, Customer>
    {
        public override void Transform(CommunicationResult apiResult, Customer contract)
        {
            var customer = contract ?? new Customer();
            customer.Communication = new Customer.Contacts
            {
                ContactId = apiResult.Id,
                Email = apiResult.Email,
                Phone = apiResult.Telephone
            };

            if (apiResult.HouseNo != null)
                customer.Communication.PostalAddress = new Customer.Contacts.Address
                {
                    HouseNo = apiResult.HouseNo,
                    City = apiResult.City,
                    Country = apiResult.Country,
                    PostalCode = apiResult.PostalCode,
                    Region = apiResult.Region
                };
        }
    }
}