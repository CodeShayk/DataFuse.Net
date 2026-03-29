using Microsoft.Extensions.Logging;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI;

public interface IWebQuery : IQuery
{
    Task<IQueryResult> Run(IHttpClientFactory httpClientFactory, ILogger? logger = null);
}
