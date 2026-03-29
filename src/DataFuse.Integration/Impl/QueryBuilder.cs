using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.Impl;

public class QueryBuilder<T> : IQueryBuilder<T> where T : IEntity
{
    private readonly IEntityConfiguration<T> entitySchema;
    private readonly ISchemaPathMatcher schemaPathMatcher;

    public QueryBuilder(IEntityConfiguration<T> entitySchema, ISchemaPathMatcher schemaPathMatcher)
    {
        this.entitySchema = entitySchema;
        this.schemaPathMatcher = schemaPathMatcher;
    }

    /// <summary>
    /// Builds a list of queries for the data entity in context based on the paths provided.
    /// </summary>
    public IQueryList Build(IDataContext context)
    {
        var mappings = entitySchema.Mappings.ToList();

        var queries = GetMappedQueries(mappings, context);

        foreach (var query in queries.Queries)
            query.ResolveQuery(context);

        return new QueryList(queries.Queries);
    }

    private QueryList GetMappedQueries(IReadOnlyCollection<Mapping<T, IQueryResult>> mappings, IDataContext context)
    {
        var queryDependencyDepth = mappings.Max(x => x.Order);

        for (var index = 1; index <= queryDependencyDepth; index++)
        {
            var maps = mappings.Where(x => x.Order == index).ToList();

            foreach (var map in maps)
            {
                var dependentQueries =
                    mappings.Where(x => x.Order == index + 1 && x.DependentOn is not null && x.DependentOn.GetType() == map.Query.GetType()).ToList();

                map.Query.Children = [];

                var filtered = FilterByPaths(context, dependentQueries);
                map.Query.Children.AddRange(filtered);
            }
        }

        var queries = FilterByPaths(context, mappings.Where(x => x.DependentOn is null))
            .Distinct(new QueryComparer())
            .ToList();

        return new QueryList(queries) { QueryDependencyDepth = queryDependencyDepth };
    }

    private IEnumerable<IQuery> FilterByPaths(IDataContext context, IEnumerable<Mapping<T, IQueryResult>> mappings)
    {
        var matchedMappings = context.Request?.SchemaPaths is not null
            ? mappings.Where(mapping => context.Request.SchemaPaths.Any(Path => schemaPathMatcher.IsMatch(Path, mapping.SchemaPaths)))
            .ToList()
            : mappings;

        return matchedMappings
        .Select(x => x.Query)
            .Distinct(new QueryComparer());
    }
}
