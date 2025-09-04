namespace Wsdot.Idl.Ifx.Filtering.v3;

public class InvalidOrderByPropertyException : Exception
{

    public InvalidOrderByPropertyException()
        : base()
    {
    }

    public InvalidOrderByPropertyException(string message)
        : base(message)
    {
    }

    public InvalidOrderByPropertyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static void ThrowIfNull(string? propertyName)
    {
        if (propertyName is null) throw new InvalidOrderByPropertyException($"{nameof(propertyName)} cannot be null.");
    }

    public static void ThrowIfNullOrWhitespace(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new InvalidOrderByPropertyException($"{nameof(propertyName)} cannot be null or whitespace.");
    }

}