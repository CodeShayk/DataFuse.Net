using System.Data;
using Dapper;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas.Queries
{
    public class CustomerQuery : SQLQuery<CustomerRecord>
    {
        protected override Func<IDbConnection, Task<CustomerRecord>> GetQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            // Executes as root or level 1 query.
            var customer = (CustomerRequest)context.Request;

            return connection => connection.QueryFirstOrDefaultAsync<CustomerRecord>(new CommandDefinition
            (
                "select CustomerId as Id, " +
                       "Customer_Name as Name," +
                       "Customer_Code as Code " +
                $"from TCustomer where customerId={customer.CustomerId}"
           ));
        }
    }
}