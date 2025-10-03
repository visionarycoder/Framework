namespace VisionaryCoder.Proxy.Abstractions;

public interface IProxyErrorClassifier
{
    ProxyErrorClassification? Classify(Exception exception);
}