using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Querying.Serialization;

public sealed class QueryFilterInterceptor : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(
        ProxyContext context,
        ProxyDelegate<T> next,
        CancellationToken cancellationToken = default)
    {
        // Example: assume filters are passed in context.Body as JSON
        if (context.Body is string json)
        {
            // Validate against schema
            QueryFilterValidator.ValidateOrThrow(json);

            // Deserialize and rehydrate
            FilterNode? node = QueryFilterSerializer.Deserialize(json);
            if (node != null && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(QueryFilter<>))
            {
                // Rehydrate into QueryFilter<TInner>
                Type innerType = typeof(T).GetGenericArguments()[0];
                var method = typeof(QueryFilterRehydrator)
                    .GetMethod(nameof(QueryFilterRehydrator.ToQueryFilter))!
                    .MakeGenericMethod(innerType);

                object rehydrated = method.Invoke(null, new object[] { node })!;
                return Response<T>.Success((T)rehydrated, 200);
            }
        }

        // Pass through if not a filter payload
        return await next(context, cancellationToken);
    }
}
