namespace VisionaryCoder.Proxy.Abstractions
{
    public partial record ProxyOptions
    {
        public int MaxRetries { get; init; } = 3;
        public TimeSpan RetryDelay { get; init; } = TimeSpan.FromSeconds(2);

        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
        public int CircuitBreakerFailures { get; init; } = 5;
        public TimeSpan CircuitBreakerDuration { get; init; } = TimeSpan.FromSeconds(30);
    }
}
