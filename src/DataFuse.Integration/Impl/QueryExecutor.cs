using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.Impl;

public class QueryExecutor : IQueryExecutor
{
    private readonly IEnumerable<IQueryEngine> queryEngines;

    public QueryExecutor(IEnumerable<IQueryEngine> queryEngines)
    {
        this.queryEngines = queryEngines;
    }

    public async Task<IList<IQueryResult>> ExecuteAsync(IDataContext context, IQueryList queries)
    {
        var globalResults = new List<IQueryResult>();
        var counter = 0;

        // supports only 5 levels of query nesting.
        while (queries.As<IQuery>().Any() && ++counter <= 5)
            queries = await ProcessAsync(context, queries, globalResults);

        return globalResults;
    }

    private async Task<IQueryList> ProcessAsync(IDataContext context, IQueryList queries, List<IQueryResult> globalResults)
    {
        var subscriber = new EventSubscriber(queries.GetChildrenQueries());
        var eventAggregator = new EventPublisher(subscriber);

        var results = await RunQueriesAsync(queries);

        CacheResults(context, results);

        eventAggregator.PublishEvent(context, new ExecutorResultArgs(results));

        globalResults.AddRange(results);

        return subscriber.ResolvedDependents;
    }

    private static void CacheResults(IDataContext context, List<IQueryResult> results)
    {
        if (context.Cache is null)
            return;

        foreach (var cacheResult in results.Where(result => result is not null && result.GetType()
                                           .GetCustomAttributes(typeof(CacheResultAttribute), false)
                                           .Any()))
            if (!context.Cache.ContainsKey(cacheResult.GetType().Name))
                context.Cache.Add(cacheResult.GetType().Name, cacheResult);
    }

    private async Task<List<IQueryResult>> RunQueriesAsync(IQueryList queryList)
    {
        var output = new List<IQueryResult>();

        foreach (var engine in queryEngines)
        {
            var tasks = queryList.Queries
                            .Where(x => engine.CanExecute(x))
                            .Select(s => s.Run(engine))
                            .ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result is not null)
                    output.Add(result);
            }
        }

        return output;
    }
}
