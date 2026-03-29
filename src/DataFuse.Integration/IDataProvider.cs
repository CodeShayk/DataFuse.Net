using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration;

public interface IDataProvider<TEntity> where TEntity : IEntity
{
    Task<TEntity> GetDataAsync(IEntityRequest request);
}
