namespace Wsdot.Idl.Ifx.Filtering.v3;

public class OrderByPropertyArgumentException : ArgumentException
{

    public OrderByPropertyArgumentException()
        : base()
    {
    }

    public OrderByPropertyArgumentException(string message)
        : base(message)
    {
    }

    public OrderByPropertyArgumentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static void ThrowIfNull(string? propertyName)
    {
        if (propertyName is null) throw new OrderByPropertyArgumentException($"{nameof(propertyName)} cannot be null.");
    }

    public static void ThrowIfNullOrWhitespace(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new OrderByPropertyArgumentException($"{nameof(propertyName)} cannot be null or whitespace.");
    }

}