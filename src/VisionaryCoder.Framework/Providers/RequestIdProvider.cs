namespace VisionaryCoder.Framework.Providers;
/// <summary>
/// Default implementation of <see cref="IRequestIdProvider"/>.
/// </summary>
public sealed class RequestIdProvider : IRequestIdProvider
{
    private static readonly AsyncLocal<string> currentRequestId = new();
    /// <inheritdoc />
    public string RequestId => currentRequestId.Value ?? GenerateNew();
    public string GenerateNew()
    {
        string newId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        currentRequestId.Value = newId;
        return newId;
    }
    public void SetRequestId(string requestId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestId);
        currentRequestId.Value = requestId;
    }
}
