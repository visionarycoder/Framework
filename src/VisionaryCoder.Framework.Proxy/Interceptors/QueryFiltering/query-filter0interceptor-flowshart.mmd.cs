flowchart LR
    subgraph Client["Client Service"]
        A[Developer builds QueryFilter<T>] --> B[Serializer → JSON payload]
    end

    subgraph ProxyPipeline["DefaultProxyPipeline"]
        B --> C[QueryFilterInterceptor]
        C -->|Valid JSON| D[Other Interceptors...]
        C -->|Invalid JSON| E[Reject 400 Bad Request]
        D --> F[HttpProxyTransport]
    end

    subgraph Server["Server Side"]
        F --> G[Schema Validator (already run in interceptor)]
        G --> H[QueryFilterRehydrator]
        H --> I[Apply to IQueryable<T>]
        I --> J[Filtered Results]
    end

    J --> Client
