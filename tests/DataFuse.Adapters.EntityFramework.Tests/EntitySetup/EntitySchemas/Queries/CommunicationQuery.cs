using Microsoft.EntityFrameworkCore;
using DataFuse.Adapters.Abstraction;
using DataFuse.Adapters.EntityFramework.Tests.Domain;

namespace DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class CommunicationQuery : SQLQuery<CommunicationRecord>
    {
        protected override Func<DbContext, Task<CommunicationRecord>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            var customer = (CustomerRecord)parentQueryResult;

            return async dbContext => await dbContext.Set<Communication>()
                .Where(p => p.Customer.Id == customer.Id)
                .Select(c => new CommunicationRecord
                {
                    Id = c.CommunicationId,
                    AddressId = c.Address.AddressId,
                    Telephone = c.Phone,
                    Email = c.Email,
                    HouseNo = c.Address.HouseNo,
                    City = c.Address.City,
                    Region = c.Address.Region,
                    PostalCode = c.Address.PostalCode,
                    Country = c.Address.Country
                })
                .FirstOrDefaultAsync();
        }
    }
}