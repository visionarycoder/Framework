namespace Wsdot.Idl.Ifx.Filtering.v3;

public class InvalidCriterionException : Exception
{

    public InvalidCriterionException()
        : base()
    {
    }

    public InvalidCriterionException(string message)
        : base(message)
    {
    }

    public InvalidCriterionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static void ThrowIfNull(string? propertyName)
    {
        if (propertyName is null) throw new InvalidCriterionException($"{nameof(propertyName)} cannot be null.");
    }

    public static void ThrowIfNullOrWhitespace(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new InvalidCriterionException($"{nameof(propertyName)} cannot be null or whitespace.");
    }

}