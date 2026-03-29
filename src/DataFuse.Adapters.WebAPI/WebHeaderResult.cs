using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI;

/// <summary>
/// Implement to return web query response with headers.
/// </summary>
public abstract class WebHeaderResult : IQueryResult
{
    public IDictionary<string, string>? Headers { get; internal set; }
}
