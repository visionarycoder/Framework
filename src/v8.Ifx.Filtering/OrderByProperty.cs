using System.ComponentModel;

namespace Wsdot.Idl.Ifx.Filtering.v3;

public record OrderByProperty(string PropertyName, ListSortDirection SortDirection = ListSortDirection.Ascending);