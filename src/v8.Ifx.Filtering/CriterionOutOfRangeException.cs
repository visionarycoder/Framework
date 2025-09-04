namespace Wsdot.Idl.Ifx.Filtering.v3;

public class CriterionOutOfRangeException : ArgumentOutOfRangeException
{
    public CriterionOutOfRangeException()
        : base()
    {

    }

    public CriterionOutOfRangeException(string message)
        : base(message)
    {

    }

    public CriterionOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}