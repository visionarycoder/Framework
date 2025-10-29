using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation;
/// <summary>
/// Default implementation of correlation context using AsyncLocal.
/// </summary>
public sealed class DefaultCorrelationContext : VisionaryCoder.Framework.Proxy.Abstractions.ICorrelationContext
{
    private static readonly AsyncLocal<string?> correlationId = new();
    /// <inheritdoc />
    public string? CorrelationId => correlationId.Value;
    public void SetCorrelationId(string correlationId)
    {
        DefaultCorrelationContext.correlationId.Value = correlationId;
    }
}
