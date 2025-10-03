namespace VisionaryCoder.Proxy.Abstractions;

public sealed class Response<T>
{

    public bool IsSuccess { get; }
    public T? Value { get; }
    public ProxyException? Error { get; }
    public string? CorrelationId { get; init; }
    public TimeSpan? Duration { get; init; }

    Response(bool isSuccess, T? value, ProxyException? error) => (IsSuccess, Value, Error) = (isSuccess, value, error);

    public static Response<T> Success(T value) => new(true, value, null);

    public static Response<T> Failure(ProxyException error) => new(false, default, error);
}