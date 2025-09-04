namespace Wsdot.Idl.Ifx.Filtering.v3;

public class PagingArgumentException : ArgumentException
{
    public PagingArgumentException()
        : base()
    {
    }
    public PagingArgumentException(string message)
        : base(message)
    {
    }
    public PagingArgumentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public static void ThrowIfExistingNotEmpty(Paging current)
    {
        if (current != Paging.Empty) throw new PagingArgumentException($"{nameof(current)} cannot be empty.");
    }
}