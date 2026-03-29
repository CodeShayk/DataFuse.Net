using DataFuse.Adapters.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using DataFuse.Integration.Impl;
using DataFuse.Integration.PathMatchers;

namespace DataFuse.Integration;

public static class ServicesExtensions
{
    public static void UseDataFuse(this IServiceCollection services, Action<IDataFuseOptions> configuration)
    {
        Guard.ThrowIfNull(configuration, nameof(configuration));
        Guard.ThrowIfNull(services, nameof(services));

        var options = new DataFuseOptionsBuilder(services);
        configuration.Invoke(options);

        if (!services.Any(s => s.ServiceType == typeof(ISchemaPathMatcher)))
            services.AddTransient<ISchemaPathMatcher, XPathMatcher>();

        if (!options.Silent && options.EntityConfigurationCount == 0)
            throw new InvalidOperationException("At least one entity configuration must be registered using WithEntityConfiguration<TEntity> method.");

        if (!options.Silent && options.EngineCount == 0)
            throw new InvalidOperationException("At least one query engine must be registered using WithEngine or WithEngines method.");

        services.AddTransient<IQueryExecutor, QueryExecutor>();
    }
}
