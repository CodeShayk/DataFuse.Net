using Microsoft.Extensions.Logging;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI;

public class QueryEngine : IQueryEngine
{
    private readonly ILogger<QueryEngine>? logger;
    private readonly IHttpClientFactory httpClientFactory;

    public QueryEngine(IHttpClientFactory httpClientFactory, ILogger<QueryEngine>? logger = null)
    {
        Guard.ThrowIfNull(httpClientFactory);
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public bool CanExecute(IQuery query) => query is IWebQuery;

    public async Task<IQueryResult> Execute(IQuery query)
    {
        if (query is not IWebQuery q)
            return null!;

        return await q.Run(httpClientFactory, logger);
    }
}
