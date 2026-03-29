using System.Data.Common;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.SQL;

public class QueryEngine : IQueryEngine
{
    private readonly SQLConfiguration sqlConfiguration;

    public QueryEngine(SQLConfiguration sqlConfiguration)
    {
        ArgumentNullException.ThrowIfNull(sqlConfiguration?.ConnectionSettings?.ProviderName,
            "SQL Configuration is required with connection settings. Provider name is missing.");

        ArgumentNullException.ThrowIfNull(sqlConfiguration?.ConnectionSettings?.ConnectionString,
            "SQL Configuration is required with connection settings. Connection string is missing.");

        this.sqlConfiguration = sqlConfiguration!;
    }

    public bool CanExecute(IQuery query) => query is ISQLQuery;

    public async Task<IQueryResult> Execute(IQuery query)
    {
        var factory = DbProviderFactories.GetFactory(sqlConfiguration.ConnectionSettings.ProviderName)
           ?? throw new InvalidOperationException($"Provider: {sqlConfiguration.ConnectionSettings.ProviderName} is not supported. Please register entry in DbProviderFactories ");

        var connection = factory.CreateConnection()
            ?? throw new InvalidOperationException($"Failed to create connection with Provider: {sqlConfiguration.ConnectionSettings.ProviderName}. Please check the connection settings.");

        await using (connection)
        {
            connection.ConnectionString = sqlConfiguration.ConnectionSettings.ConnectionString;
            return await ((ISQLQuery)query).Run(connection);
        }
    }
}
