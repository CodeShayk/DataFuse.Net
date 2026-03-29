namespace DataFuse.Adapters.Abstraction;

public interface ISchemaPathMatcher
{
    /// <summary>
    /// Determines whether there is a match for given input path vs configured paths for entity's object graph.
    /// </summary>
    bool IsMatch(string inputPath, ISchemaPaths configuredPaths);
}
