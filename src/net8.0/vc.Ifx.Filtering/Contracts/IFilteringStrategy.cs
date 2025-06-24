using Ifx.Filtering;

namespace v8.Ifx.Data.Filtering.Contracts;

public interface IFilteringStrategy<T> where T : class
{
    IQueryable<T> ApplyFiltering(IQueryable<T> query, Filter<T> filter);
}

public interface IFilteringStrategy
{
    IQueryable<T> ApplyFiltering<T>(IQueryable<T> query, Filter filter) where T : class;

}