namespace Wsdot.Idl.Ifx.Filtering.v3;

public class PagingOutOfRangeException : ArgumentOutOfRangeException
{
    public PagingOutOfRangeException()
        : base()
    {

    }

    public PagingOutOfRangeException(string message)
        : base(message)
    {

    }

    public PagingOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public static void ThrowIfLessThanZero(int item)
    {
        if (item < 0) throw new PagingOutOfRangeException($"{nameof(item)} cannot be less than zero.");
    }

    public static void ThrowIfLessThanZero(int? item)
    {
        if (item is null) return;
        if (item < 0) throw new PagingOutOfRangeException($"{nameof(item)} cannot be less than zero.");
    }

    public static void ThrowIfOutOfRange(int? skip, int? take)
    {
        ThrowIfLessThanZero(skip);
        ThrowIfLessThanZero(take);
    }

}