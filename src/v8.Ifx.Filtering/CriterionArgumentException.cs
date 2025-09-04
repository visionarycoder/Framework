namespace Wsdot.Idl.Ifx.Filtering.v3;

public partial class CriterionArgumentException : ArgumentException
{
    public CriterionArgumentException()
        : base()
    {
    }

    public CriterionArgumentException(string message)
        : base(message)
    {
    }

    public CriterionArgumentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static void ThrowIfNull(string? propertyName)
    {
        if (propertyName is null) throw new CriterionArgumentException($"{nameof(propertyName)} cannot be null.");
    }

    public static void ThrowIfNullOrWhitespace(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new CriterionArgumentException($"{nameof(propertyName)} cannot be null or whitespace.");
    }
}