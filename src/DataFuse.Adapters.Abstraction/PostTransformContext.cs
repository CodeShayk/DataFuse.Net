namespace DataFuse.Adapters.Abstraction;

public class PostTransformContext : TransformContext
{
    /// <summary>
    /// Indicates whether the transformation needs to be repeated.
    /// </summary>
    internal bool IsRepeat { get; private set; }

    /// <summary>
    /// Repeat the transformation process.
    /// </summary>
    public void Repeat() => IsRepeat = true;

    /// <summary>
    /// Creates a new instance of PostTransformContext with the provided parameters.
    /// </summary>
    public PostTransformContext(TransformContext context) :
        base(context.DataContext, context.QueryResult, context.Transformer, context.Entity, context.IsRepeated)
    {
    }
}
