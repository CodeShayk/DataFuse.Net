namespace DataFuse.Adapters.Abstraction;

public class ChildrenQueries
{
    public Type ParentQueryResultType { get; set; } = null!;
    public IList<IQuery> Queries { get; set; } = [];
}
