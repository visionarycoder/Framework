namespace VisionaryCoder.Proxy.Abstractions;

public delegate Task<Response<T>> ProxyDelegate<T>(ProxyContext context);