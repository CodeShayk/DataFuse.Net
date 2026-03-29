using DataFuse.Integration.Helpers;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.PathMatchers;

public class JPathMatcher : ISchemaPathMatcher
{
    public bool IsMatch(string inputXPath, ISchemaPaths configuredXPaths) =>
          inputXPath.IsNotNullOrEmpty()
          && configuredXPaths.Paths.Any(x => inputXPath.Contains(x, StringComparison.OrdinalIgnoreCase));
}
