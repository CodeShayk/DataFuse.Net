using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.Impl;

public class EventSubscriber : ISubscriber<ExecutorResultArgs>
{
    private readonly IList<ChildrenQueries> dependentQueries;

    public EventSubscriber(IList<ChildrenQueries> dependentQueries)
    {
        this.dependentQueries = GetUniqueQueryList(dependentQueries);
    }

    public void OnEventHandler(IDataContext context, ExecutorResultArgs args)
    {
        if (args.Result is null)
            return;

        var results = args.Result;

        foreach (var queryResult in results.Where(x => !typeof(IPolymorphicResult).IsAssignableFrom(x.GetType())))
        {
            if (dependentQueries.All(tuple => tuple.ParentQueryResultType != queryResult.GetType()))
                continue;

            var result = queryResult;

            foreach (var unresolved in dependentQueries)
            {
                if (unresolved.ParentQueryResultType != result.GetType())
                    continue;

                foreach (var query in unresolved.Queries)
                    query.ResolveQuery(context, queryResult);
            }
        }

        foreach (var queryResult in results.Where(x => typeof(IPolymorphicResult).IsAssignableFrom(x.GetType())))
        {
            if (dependentQueries.All(tuple => tuple.ParentQueryResultType != queryResult.GetType() &&
                !tuple.ParentQueryResultType.IsAssignableFrom(queryResult.GetType()))
            )
                continue;

            var result = queryResult;

            foreach (var unresolved in dependentQueries)
            {
                if (unresolved.ParentQueryResultType != result.GetType() &&
                    !unresolved.ParentQueryResultType.IsAssignableFrom(result.GetType()))
                    continue;

                foreach (var query in unresolved.Queries)
                    query.ResolveQuery(context, queryResult);
            }
        }
    }

    public QueryList ResolvedDependents => new(dependentQueries
            .SelectMany(x => x.Queries)
            .Where(query => query.IsContextResolved()));

    private static IList<ChildrenQueries> GetUniqueQueryList(IList<ChildrenQueries> queries)
    {
        if (queries is null)
            return new List<ChildrenQueries>();

        var distincts = queries
            .Select(x => x.ParentQueryResultType)
            .Distinct()
            .Select(x =>
            {
                var distinctMerge = queries
                    .Where(d => d.ParentQueryResultType == x)
                    .SelectMany(q => q.Queries)
                    .Distinct(new QueryComparer())
                    .ToList();

                return new ChildrenQueries { ParentQueryResultType = x, Queries = distinctMerge };
            })
            .ToList();

        return distincts;
    }
}
