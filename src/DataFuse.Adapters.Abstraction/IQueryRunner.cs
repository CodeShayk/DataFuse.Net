namespace DataFuse.Adapters.Abstraction;

public interface IQueryRunner
{
    Task<IQueryResult> Run(IQueryEngine engine);
}
