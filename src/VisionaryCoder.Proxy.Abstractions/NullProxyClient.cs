namespace VisionaryCoder.Proxy.Abstractions;

public sealed class NullProxyClient : IProxyClient
{
    public Task<Response<T>> SendAsync<T>(object request, CancellationToken ct = default) => Task.FromResult(Response<T>.Failure(new BusinessException("Null proxy in use")));
}