using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration;

public interface IQueryBuilder<T>
{
    IQueryList Build(IDataContext context);
}
