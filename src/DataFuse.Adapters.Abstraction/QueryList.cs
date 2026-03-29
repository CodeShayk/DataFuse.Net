namespace DataFuse.Adapters.Abstraction;

public class QueryList : IQueryList
{
    private readonly List<IQuery> queryList;

    public QueryList()
    {
        queryList = [];
    }

    public IEnumerable<IQuery> Queries => queryList;

    public QueryList(IEnumerable<IQuery> collection)
    {
        queryList = new List<IQuery>(collection);
    }

    public int QueryDependencyDepth { get; set; }

    public IQueryList GetByType<T>() where T : class
    {
        var queries = queryList.Where(q => q as T is not null);
        return new QueryList(queries);
    }

    public List<T> As<T>() => queryList.Cast<T>().ToList();

    public List<ChildrenQueries> GetChildrenQueries()
    {
        var childrenQueries = queryList
            .Select(x => new ChildrenQueries { ParentQueryResultType = x.ResultType, Queries = x.Children })
            .Where(x => x.Queries is not null && x.Queries.Any())
            .ToList();

        return childrenQueries
            .Select(x =>
            {
                var distinctList = childrenQueries
                    .Where(d => d.ParentQueryResultType == x.ParentQueryResultType)
                    .SelectMany(q => q.Queries)
                    .Distinct(new QueryComparer())
                    .ToList();

                return new ChildrenQueries { ParentQueryResultType = x.ParentQueryResultType, Queries = distinctList };
            })
            .ToList();
    }

    public new int Count() => queryList.Count;

    public bool IsEmpty() => queryList.Count == 0;

    public void AddRange(IEnumerable<IQuery> collection)
    {
        queryList.AddRange(collection);
    }
}
