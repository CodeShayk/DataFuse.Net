using Microsoft.Extensions.Logging;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Adapters.WebAPI;

public abstract class WebQuery<TQueryResult> : BaseQuery<TQueryResult>, IWebQuery
      where TQueryResult : IQueryResult
{
    protected Uri? BaseAddress;

    protected WebQuery() : this(string.Empty)
    {
    }

    protected WebQuery(string baseAddress)
    {
        if (!string.IsNullOrEmpty(baseAddress))
            BaseAddress = new Uri(baseAddress);
    }

    private Func<Uri>? UriDelegate = null;

    public override bool IsContextResolved() => UriDelegate is not null;

    public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        UriDelegate = GetQuery(context, parentQueryResult);
    }

    /// <summary>
    /// Override to pass custom outgoing headers with the api request.
    /// </summary>
    protected virtual IDictionary<string, string> GetRequestHeaders()
    {
        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Override to get custom incoming headers with the api response.
    /// The headers collection will be present on `WebHeaderResult.Headers` when api response includes any of the headers defined in this method.
    /// </summary>
    protected virtual IEnumerable<string> GetResponseHeaders()
    {
        return [];
    }

    /// <summary>
    /// Implement to construct the api web query.
    /// </summary>
    /// <param name="context">Request Context. Always available.</param>
    /// <param name="parentApiResult">Result from parent Query. Only available when configured as nested web query. Else will be null.</param>
    protected abstract Func<Uri> GetQuery(IDataContext context, IQueryResult? parentApiResult = null);

    async Task<IQueryResult> IWebQuery.Run(IHttpClientFactory httpClientFactory, ILogger? logger)
    {
        Guard.ThrowIfNull(httpClientFactory);

        logger?.LogInformation("Run api: {QueryName}", GetType().Name);

        var uri = UriDelegate!();

        if (uri is null)
            return null!;

        using var client = httpClientFactory.CreateClient();

        logger?.LogInformation("Executing web api on thread {ThreadId} (task {TaskId})",
            Thread.CurrentThread.ManagedThreadId, Task.CurrentId);

        try
        {
            try
            {
                if (BaseAddress is not null)
                    client.BaseAddress = BaseAddress;

                var requestHeaders = GetRequestHeaders();

                if (requestHeaders is { Count: > 0 })
                    foreach (var header in requestHeaders)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var result = await client.GetAsync(uri);

                var raw = await result.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(raw))
                    logger?.LogInformation("Result.Content of executing web api: {Path} is {Content}", uri.AbsolutePath, raw);

                if (!result.IsSuccessStatusCode)
                {
                    logger?.LogInformation("Result of executing web api {Path} is not success status code", uri.AbsolutePath);
                    return null!;
                }

                if (typeof(TQueryResult).UnderlyingSystemType?.Name.Equals(typeof(CollectionResult<>).Name) == true)
                {
                    var typeArgs = typeof(TQueryResult).GetGenericArguments();
                    var arrType = typeArgs[0].MakeArrayType();
                    var arrObject = System.Text.Json.JsonSerializer.Deserialize(raw, arrType);
                    if (arrObject is not null)
                    {
                        var resultType = typeof(CollectionResult<>);
                        var collectionType = resultType.MakeGenericType(typeArgs);
                        var collectionResult = (TQueryResult)Activator.CreateInstance(collectionType, arrObject)!;

                        SetResponseHeaders(result, collectionResult);

                        return collectionResult;
                    }
                }
                else
                {
                    var obj = System.Text.Json.JsonSerializer.Deserialize(raw, typeof(TQueryResult));
                    if (obj is not null)
                    {
                        var resObj = (TQueryResult)obj;
                        SetResponseHeaders(result, resObj);
                        return resObj;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                logger?.LogWarning(ex, "An error occurred while sending the request. Query URL: {Path}", uri.AbsolutePath);
            }
            catch (HttpRequestException ex)
            {
                logger?.LogWarning(ex, "An error occurred while sending the request. Query URL: {Path}", uri.AbsolutePath);
            }
        }
        catch (AggregateException ex)
        {
            logger?.LogInformation("Web api {QueryName} failed", GetType().Name);
            foreach (var e in ex.InnerExceptions)
                logger?.LogError(e, "");
        }

        return null!;
    }

    private void SetResponseHeaders(HttpResponseMessage response, TQueryResult result)
    {
        if (response.Headers is null || result is null)
            return;

        var headers = GetResponseHeaders();

        if (headers is null || !headers.Any())
            return;

        if (result is not WebHeaderResult webResult)
            throw new InvalidOperationException($"{typeof(TQueryResult).Name} should implement from WebHeaderResult for response Headers");

        foreach (var header in headers)
        {
            if (!response.Headers.Any(r => r.Key == header))
                continue;

            var responseHeader = response.Headers.First(r => r.Key == header);

            var value = responseHeader.Value is not null && responseHeader.Value.Any()
                                        ? responseHeader.Value.First()
                                        : string.Empty;

            webResult.Headers ??= new Dictionary<string, string>();

            webResult.Headers.Add(responseHeader.Key, value);
        }
    }
}
