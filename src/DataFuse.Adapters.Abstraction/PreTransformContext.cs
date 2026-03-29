namespace DataFuse.Adapters.Abstraction;

public class PreTransformContext : TransformContext
{
    /// <summary>
    /// Indicates whether the transformation has been cancelled.
    /// </summary>
    internal bool IsCancelled { get; private set; }

    /// <summary>
    /// Cancels the transformation process.
    /// </summary>
    public void Cancel() => IsCancelled = true;

    /// <summary>
    /// Creates a new instance of PreTransformContext with the provided parameters.
    /// </summary>
    public PreTransformContext(TransformContext context) :
        base(context.DataContext, context.QueryResult, context.Transformer, context.Entity, context.IsRepeated)
    {
    }
}
