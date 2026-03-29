namespace DataFuse.Adapters.Abstraction;

/// <summary>
/// Transformer hooks interface to allow pre and post transformation actions.
/// </summary>
public interface ITransformerHooks
{
    /// <summary>
    /// Pre-transform hook to perform actions before the transformation.
    /// </summary>
    void PreTransform(PreTransformContext context);

    /// <summary>
    /// Post-transform hook to perform actions after the transformation.
    /// </summary>
    void PostTransform(PostTransformContext context);
}
