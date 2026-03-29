using System.Data;
using Dapper;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries
{
    internal class CommunicationQuery : SQLQuery<CommunicationRecord>
    {
        protected override Func<IDbConnection, Task<CommunicationRecord>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Execute as child to customer query.
            var customer = (CustomerRecord)parentQueryResult;

            return connection => connection.QueryFirstOrDefaultAsync<CommunicationRecord>(new CommandDefinition
            (
                "select c.CommunicationId as ContactId, " +
                       "c.Phone as Telephone, " +
                       "c.Email, " +
                       "a.AddressId, " +
                       "a.HouseNo, " +
                       "a.City, " +
                       "a.Region, " +
                       "a.PostCode as PostalCode, " +
                       "a.Country " +
                "from TCommunication c " +
                "left join TAddress a on a.CommunicationId = c.CommunicationId " +
                $"where customerId={customer.Id}"
           ));
        }
    }
}