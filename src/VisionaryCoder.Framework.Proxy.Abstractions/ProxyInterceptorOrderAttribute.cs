namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Attribute to specify the execution order of proxy interceptors.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class ProxyInterceptorOrderAttribute : Attribute
{
    /// <summary>
    /// Gets the order value for the interceptor.
    /// Lower values execute first.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyInterceptorOrderAttribute"/> class.
    /// </summary>
    /// <param name="order">The order value. Lower values execute first.</param>
    public ProxyInterceptorOrderAttribute(int order)
    {
        Order = order;
    }
}
