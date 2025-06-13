using System.ComponentModel.DataAnnotations;
using Ifx.Filtering;

namespace v8.Ifx.Data.Filtering.Extensions;

/// <summary>
///     Extensions for configuring and applying filters.
/// </summary>
public static class FilterExtensions
{

       /// <summary>
    ///     Validates the filter by checking if the property names in the criteria exist in the entity type.
    /// </summary>
    /// <typeparam name="T">The type of the entity being filtered.</typeparam>
    /// <param name="filter">The filter to validate.</param>
    /// <returns>A ValidationResult object indicating whether the filter is valid or not.</returns>
    public static ValidationResult ValidateFilter<T>(this Filter<T> filter) where T : class
    {
        // Get the set of property names for the entity type T
        var properties = typeof(T).GetProperties().Select(p => p.Name).ToHashSet();

        // Find criteria with invalid property names
        var invalidProperties = filter.Criteria
            .Where(criterion => !properties.Contains(criterion.PropertyName))
            .Select(criterion => criterion.PropertyName)
            .ToList();

        // Return a ValidationResult indicating success or failure
        return invalidProperties.Any()
            ? new ValidationResult("Filter validation failed.", invalidProperties)
            : ValidationResult.Success!;
    }

    /// <summary>
    ///     Converts a filter of one type to a filter of another type.
    /// </summary>
    /// <typeparam name="TSource">The source type of the filter.</typeparam>
    /// <typeparam name="TDestination">The destination type of the filter.</typeparam>
    /// <param name="source">The source filter to convert.</param>
    /// <returns>A new filter of the destination type.</returns>
    public static Filter<TDestination> Convert<TSource, TDestination>(this Filter<TSource> source) where TSource : class where TDestination : class
    {
        var target = new Filter<TDestination>()
        {
            Skip = source.Skip,
            Take = source.Take,
        };
        target.AddCriteria(source.Criteria.ToArray());
        target.AddOrderByClauses(source.OrderBy.ToList());
        return target;
    }
}