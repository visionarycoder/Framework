using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy
{

    public sealed class ProxyClient(IProxyErrorClassifier classifier, ProxyOptions options) : IProxyClient
    {
        public async Task<Response<T>> SendAsync<T>(object request, CancellationToken ct = default)
        {
            try
            {
                // TODO: real transport call
                await Task.Delay(10, ct);
                return Response<T>.Success(default!);
            }
            catch (ProxyException pe) // already a proxy-defined error
            {
                return Response<T>.Failure(pe);
            }
            catch (Exception ex) // normalize everything else
            {
                var kind = classifier.Classify(ex);
                var normalized = kind switch
                {
                    ProxyErrorClassification.Transient => new RetryableTransportException(ex.Message, ex),
                    ProxyErrorClassification.NonTransient => new NonRetryableTransportException(ex.Message, ex),
                    ProxyErrorClassification.Business => new BusinessException(ex.Message, ex),
                    _ => new NonRetryableTransportException("Unhandled proxy exception", ex)
                };
                return Response<T>.Failure(normalized);
            }
        }
    }
}
