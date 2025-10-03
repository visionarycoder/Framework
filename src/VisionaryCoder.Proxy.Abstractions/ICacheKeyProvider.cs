namespace VisionaryCoder.Proxy.Abstractions;

public interface ICacheKeyProvider
{
    string? GetKey(object request, Type resultType);
}