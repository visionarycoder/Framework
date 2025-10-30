namespace VisionaryCoder.Framework.Tests.Extensions;

public class TestClass : IDisposable
{
    public string GetValue()
    {
        return "TestValue";
    }

    public void ThrowException()
    {
        throw new InvalidOperationException("Test exception");
    }

    public string OverloadedMethod()
    {
        return "NoParam";
    }

    public string OverloadedMethod(string param)
    {
        return $"WithParam:{param}";
    }

    public void Dispose()
    {
        // Test implementation
    }
}
