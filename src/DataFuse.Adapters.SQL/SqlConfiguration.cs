namespace DataFuse.Adapters.SQL;

public class SQLConfiguration
{
    public SQLConfiguration()
    {
        ConnectionSettings = new ConnectionSettings();
        QuerySettings = new QuerySettings();
    }

    public ConnectionSettings ConnectionSettings { get; set; }
    public QuerySettings QuerySettings { get; set; }
}

public class ConnectionSettings
{
    public string ProviderName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
}

public class QuerySettings
{
    public int QueryBatchSize { get; set; } = 10;
}
