using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.Impl;

public class EntityBuilder<T> : IEntityBuilder<T> where T : IEntity, new()
{
    private readonly IEntityConfiguration<T> entitySchema;

    public EntityBuilder(IEntityConfiguration<T> entitySchema)
    {
        this.entitySchema = entitySchema;
    }

    /// <summary>
    /// Executes configured transformers to populate target entity using list of query results.
    /// </summary>
    public T Build(IDataContext context, IList<IQueryResult> queryResults)
    {
        var entity = new T();

        if (queryResults is null || queryResults.Count == 0)
            return entity;

        var mappings = entitySchema.Mappings.ToList();

        // resolve context of each transformer so it is available inside for transformation if required.
        mappings.ForEach(mapping => (mapping.Transformer as ITransformerContext)?.SetContext(context));

        var queryDependencyDepth = mappings.Max(x => x.Order);

        // iterate through transformers to build data source by order configured.
        for (var index = 1; index <= queryDependencyDepth; index++)
        {
            var transformers = mappings
                .Where(mapping => mapping.Order == index)
                .Select(m => m.Transformer)
                .Distinct()
                .ToList();

            foreach (var queryResult in queryResults)
                transformers.Where(transformer => IsMatch(((ITransformerQueryResult)transformer).SupportedQueryResult, queryResult.GetType())).ToList()
                    .ForEach(transformer => TransformWithHooks(new TransformContext(context, queryResult, transformer, entity, false)));
        }

        return entity;
    }

    private static bool IsMatch(Type transformerQueryResult, Type queryResult)
    {
        return transformerQueryResult == queryResult;
    }

    /// <summary>
    /// Transform method that includes hooks for pre and post transformation.
    /// </summary>
    private static void TransformWithHooks(TransformContext context)
    {
        // Create a pre-transform context to pass the data before transformation.
        var preTransformContext = new PreTransformContext(context);
        // Call the pre-transform method with the context.
        if (context.Transformer is ITransformerHooks transformerPreHook)
            transformerPreHook.PreTransform(preTransformContext);

        // If the pre-transform context is cancelled, skip the transformation.
        if (!preTransformContext.IsCancelled)
            context.Transformer.Transform(context.QueryResult, context.Entity);

        // Post-transform hook can be used to perform any post-transformation logic if needed.
        if (!preTransformContext.IsCancelled && context.Transformer is ITransformerHooks transformerPostHook)
        {
            // Create a post-transform context to pass the data after transformation.
            var postTransformContext = new PostTransformContext(context);
            // Call the post-transform method with the context.
            transformerPostHook.PostTransform(postTransformContext);
            // If the post-transform context is repeat, transform again.
            if (!postTransformContext.IsRepeated && postTransformContext.IsRepeat)
            {
                // Mark as repeated to avoid infinite loop.
                postTransformContext.IsRepeated = true;
                // Repeat the transformation process.
                TransformWithHooks(postTransformContext);
            }
        }
    }
}
