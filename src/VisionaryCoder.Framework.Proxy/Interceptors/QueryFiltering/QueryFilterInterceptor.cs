using VisionaryCoder.Framework.Proxy.Abstractions;using VisionaryCoder.Framework.Proxy.Abstractions;using VisionaryCoder.Framework.Proxy.Abstractions;



namespace VisionaryCoder.Framework.Proxy.Interceptors.QueryFiltering;using VisionaryCoder.Framework.Querying.Serialization;



public sealed class QueryFilterInterceptor : IProxyInterceptornamespace VisionaryCoder.Framework.Proxy.Interceptors.QueryFiltering;

{

    public async Task<Response<T>> InvokeAsync<T>(namespace VisionaryCoder.Framework.Proxy.Interceptors.QueryFiltering;

        ProxyContext context,

        ProxyDelegate<T> next,public sealed class QueryFilterInterceptor : IProxyInterceptor

        CancellationToken cancellationToken = default)

    {{public sealed class QueryFilterInterceptor : IProxyInterceptor

        // Placeholder for query filter processing

        // TODO: Implement query filter validation and transformation    public async Task<Response<T>> InvokeAsync<T>({

        

        return await next(cancellationToken);        ProxyContext context,    public async Task<Response<T>> InvokeAsync<T>(

    }

}        ProxyDelegate<T> next,        ProxyContext context,

        CancellationToken cancellationToken = default)        ProxyDelegate<T> next,

    {        CancellationToken cancellationToken = default)

        // Placeholder for query filter processing    {

        // TODO: Implement query filter validation and transformation        // Example: assume filters are passed in context.Body as JSON

                if (context.Body is string json)

        return await next(cancellationToken);        {

    }            // Validate against schema

}            QueryFilterValidator.ValidateOrThrow(json);

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
