namespace Wsdot.Idl.Ifx.Filtering.v3;

public class OrderByPropertyOutOfRangeException : ArgumentOutOfRangeException
{

    public OrderByPropertyOutOfRangeException()
        : base()
    {
    }

    public OrderByPropertyOutOfRangeException(string message)
        : base(message)
    {
    }

    public OrderByPropertyOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}