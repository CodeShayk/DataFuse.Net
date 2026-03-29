namespace DataFuse.Adapters.Abstraction;

public interface IDataContext : IEntityContextCache
{
    IEntityRequest Request { get; }
}
