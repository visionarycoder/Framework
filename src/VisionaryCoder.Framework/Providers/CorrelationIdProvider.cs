namespace VisionaryCoder.Framework;

/// <summary>
/// Default implementation of <see cref="ICorrelationIdProvider"/>.
/// </summary>
public sealed class CorrelationIdProvider : ICorrelationIdProvider

{
    private static readonly AsyncLocal<string> currentCorrelationId = new();

    /// <inheritdoc />
    public string CorrelationId => currentCorrelationId.Value ?? GenerateNew();

    /// <inheritdoc />
    public string GenerateNew()
    {
        var newId = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
        currentCorrelationId.Value = newId;
        return newId;
    }

    /// <inheritdoc />
    public void SetCorrelationId(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        currentCorrelationId.Value = correlationId;
    }
}