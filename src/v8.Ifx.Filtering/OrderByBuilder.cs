using System.ComponentModel;

namespace Wsdot.Idl.Ifx.Filtering.v3;

public class OrderByBuilder
{

    private string propertyNameImp = string.Empty;
    private ListSortDirection sortDirectionImp = ListSortDirection.Ascending;

    public OrderByBuilder ForProperty(string propertyName)
    {
        propertyNameImp = propertyName;
        return this;
    }

    public OrderByBuilder Ascending()
    {
        sortDirectionImp = ListSortDirection.Ascending;
        return this;
    }

    public OrderByBuilder Descending()
    {
        sortDirectionImp = ListSortDirection.Descending;
        return this;
    }

    public OrderByBuilder WithSortDirection(ListSortDirection sortDirection)
    {
        sortDirectionImp = sortDirection;
        return this;
    }

    public OrderByProperty Build()
    {
        return new OrderByProperty(propertyNameImp, sortDirectionImp);
    }
}