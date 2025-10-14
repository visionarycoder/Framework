namespace VisionaryCoder.Framework;

/// <summary>
/// Default implementation of <see cref="IRequestIdProvider"/>.
/// </summary>
public sealed class RequestIdProvider : IRequestIdProvider
{
    private static readonly AsyncLocal<string> currentRequestId = new();

    /// <inheritdoc />
    public string RequestId => currentRequestId.Value ?? GenerateNew();

    /// <inheritdoc />
    public string GenerateNew()
    {
        var newId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        currentRequestId.Value = newId;
        return newId;
    }

    /// <inheritdoc />
    public void SetRequestId(string requestId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestId);
        currentRequestId.Value = requestId;
    }
}