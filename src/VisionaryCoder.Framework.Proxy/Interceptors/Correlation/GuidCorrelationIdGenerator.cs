using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation;
/// <summary>
/// Default correlation ID generator that creates GUIDs.
/// </summary>
public sealed class GuidCorrelationIdGenerator : VisionaryCoder.Framework.Proxy.Abstractions.ICorrelationIdGenerator
{
    /// <inheritdoc />
    public string GenerateCorrelationId()
    {
        return Guid.NewGuid().ToString("D");
    }
}
