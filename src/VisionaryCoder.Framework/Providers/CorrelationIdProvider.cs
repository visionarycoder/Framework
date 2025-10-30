using System;
using System.Threading;

namespace VisionaryCoder.Framework.Providers;

/// <summary>
/// Default implementation of <see cref="ICorrelationIdProvider"/>.
/// </summary>
public sealed class CorrelationIdProvider : ICorrelationIdProvider
{
    private static readonly AsyncLocal<string?> currentCorrelationId = new();

    /// <inheritdoc />
    public string CorrelationId => currentCorrelationId.Value ?? GenerateNew();

    public string GenerateNew()
    {
        string newId = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
        currentCorrelationId.Value = newId;
        return newId;
    }

    public void SetCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            throw new ArgumentException("Correlation ID cannot be null or whitespace.", nameof(correlationId));
        }
        currentCorrelationId.Value = correlationId;
    }
}
