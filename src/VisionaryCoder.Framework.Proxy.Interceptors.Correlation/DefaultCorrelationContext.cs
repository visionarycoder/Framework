namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation;

/// <summary>
/// Default implementation of correlation context using AsyncLocal.
/// </summary>
public sealed class DefaultCorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<string?> correlationId = new();

    /// <inheritdoc />
    public string? CorrelationId => correlationId.Value;

    /// <inheritdoc />
    public void SetCorrelationId(string correlationId)
    {
        DefaultCorrelationContext.correlationId.Value = correlationId;
    }
}