namespace DataFuse.Adapters.Abstraction;

public abstract class BaseTransformer<TQueryResult, TEntity> : ITransformer, ITransformerContext, ITransformerQueryResult, ITransformerHooks
    where TEntity : IEntity
    where TQueryResult : IQueryResult
{
    /// <summary>
    /// Transformer instance of data context.
    /// </summary>
    protected IDataContext? Context { get; private set; }

    /// <summary>
    /// Supported QueryResult type for the transformer.
    /// </summary>
    public Type SupportedQueryResult => typeof(TQueryResult);

    /// <summary>
    /// Method to set data context for the transformer
    /// </summary>
    public void SetContext(IDataContext context) => Context = context;

    /// <summary>
    /// Transform method mapping query result data to entity.
    /// </summary>
    public void Transform(IQueryResult queryResult, IEntity entity)
    {
        Transform((TQueryResult)queryResult, (TEntity)entity);
    }

    /// <summary>
    /// Implement this method to map data to entity.
    /// </summary>
    public abstract void Transform(TQueryResult queryResult, TEntity entity);

    /// <summary>
    /// Pre-transform method that can be used to perform any pre-transformation logic if needed.
    /// </summary>
    public virtual void PreTransform(PreTransformContext context)
    {
    }

    /// <summary>
    /// Post-transform method that can be used to perform any post-transformation logic if needed.
    /// </summary>
    public virtual void PostTransform(PostTransformContext context)
    {
    }
}
