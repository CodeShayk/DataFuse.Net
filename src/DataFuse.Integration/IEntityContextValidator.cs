using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration;

public interface IEntityContextValidator
{
    void Validate(IEntityRequest context);
}
