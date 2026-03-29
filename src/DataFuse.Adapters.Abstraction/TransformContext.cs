namespace DataFuse.Adapters.Abstraction;

public class TransformContext
{
    /// <summary>
    /// Context for the data being transformed. Has cached results and other data.
    /// </summary>
    public IDataContext DataContext { get; private set; }

    /// <summary>
    /// Type of the transformer that will be used for transformation.
    /// </summary>
    public Type? TransformerType => Transformer?.GetType();

    public ITransformer Transformer { get; private set; }

    /// <summary>
    /// Entity that is being transformed.
    /// </summary>
    public IEntity Entity { get; private set; }

    /// <summary>
    /// Query result (data) that is being used for transformation
    /// </summary>
    public IQueryResult QueryResult { get; private set; }

    /// <summary>
    /// Indicates whether the transformation has been repeated.
    /// </summary>
    internal bool IsRepeated { get; set; }

    /// <summary>
    /// Creates a new instance of TransformContext with the provided parameters.
    /// </summary>
    public TransformContext(IDataContext dataContext, IQueryResult queryResult, ITransformer transformer, IEntity entity, bool isRepeated)
    {
        DataContext = dataContext;
        QueryResult = queryResult;
        Transformer = transformer;
        Entity = entity;
        IsRepeated = isRepeated;
    }
}
