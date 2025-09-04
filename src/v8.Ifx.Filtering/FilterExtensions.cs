//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection;

//namespace Wsdot.Idl.Ifx.Filtering.v3;

//public static class FilterExtensions
//{

//    public static bool Add(this Filter filter, Criterion item)
//    {
//        InvalidCriterionException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.Criteria.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        if (matching is not null)
//            throw new CriterionArgumentException("Unable to add item.  Item already exists in the collection.");
//        filter.Criteria.Add(item);
//        return true;
//    }

//    public static bool Add(this Filter filter, OrderByProperty item)
//    {
//        InvalidOrderByPropertyException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.OrderBy.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        if (matching is null)
//        {
//            filter.OrderBy.Add(item);
//            return true;
//        }
//        throw new OrderByPropertyArgumentException("Unable to add item.");
//    }

//    public static bool Add(this Filter filter, Paging item)
//    {

//        PagingOutOfRangeException.ThrowIfLessThanZero(item.Skip);
//        PagingOutOfRangeException.ThrowIfLessThanZero(item.Take);
//        PagingArgumentException.ThrowIfExistingNotEmpty(filter.Paging);
//        if (filter.Paging == Paging.Empty)
//        {
//            filter.Paging = item;
//            return true;
//        }
//        return false;

//    }

//    public static bool AddCriterion(this Filter filter, string propertyName, object? propertyValue, ComparisonType comparisonType = ComparisonType.Equals, IgnoreCase ignoreCase = IgnoreCase.No)
//    {
//        var item = new Criterion(propertyName, propertyValue, comparisonType, ignoreCase);
//        return filter.Add(item);
//    }

//    public static bool AddOrderByProperty(this Filter filter, string propertyName, ListSortDirection sortDirection = ListSortDirection.Ascending)
//    {
//        var item = new OrderByProperty(propertyName, sortDirection);
//        return filter.Add(item);
//    }

//    public static bool AddPaging(this Filter filter, int skip, int? take)
//    {
//        var paging = new Paging(skip, take);
//        return filter.Add(paging);
//    }

//    public static bool AddOrUpdate(this Filter filter, Criterion item)
//    {
//        InvalidCriterionException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.Criteria.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        return matching is null
//            ? filter.Add(item)
//            : filter.Update(item);
//    }

//    public static bool AddOrUpdate(this Filter filter, OrderByProperty item)
//    {
//        InvalidOrderByPropertyException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.OrderBy.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        return matching is null
//            ? filter.Add(item)
//            : filter.Update(item);
//    }

//    public static bool AddOrUpdate(this Filter filter, Paging item)
//    {
//        return (filter.Paging == Paging.Empty)
//            ? filter.Add(item)
//            : filter.Update(item);
//    }

//    public static bool AddOrUpdateCriterion(this Filter filter, string propertyName, object? propertyValue, ComparisonType comparisonType = ComparisonType.Equals, IgnoreCase ignoreCase = IgnoreCase.No)
//    {
//        var item = new Criterion(propertyName, propertyValue, comparisonType, ignoreCase);
//        return filter.Update(item);
//    }

//    public static bool AddOrUpdateOrderByProperty(this Filter filter, string propertyName, ListSortDirection sortDirection = ListSortDirection.Ascending)
//    {
//        var item = new OrderByProperty(propertyName, sortDirection);
//        return filter.Update(item);
//    }

//    public static bool AddOrUpdatePaging(this Filter filter, int skip, int? take)
//    {
//        var item = new Paging(skip, take);
//        return filter.AddOrUpdate(item);
//    }
//    public static bool Update(this Filter filter, Criterion item)
//    {
//        InvalidCriterionException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.Criteria.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        if (matching is null)
//        {
//            throw new CriterionOutOfRangeException("Unable to add item.  Item already exists in the collection.");
//        }
//        var idx = filter.Criteria.IndexOf(matching);
//        filter.Criteria.Remove(matching);
//        filter.Criteria.Insert(idx, item);
//        return true;
//    }

//    public static bool Update(this Filter filter, OrderByProperty item)
//    {
//        InvalidOrderByPropertyException.ThrowIfNullOrWhitespace(item.PropertyName);
//        var matching = filter.OrderBy.SingleOrDefault(c => c.PropertyName == item.PropertyName);
//        if (matching is null)
//        {
//            throw new OrderByPropertyOutOfRangeException("Item not found");
//        }
//        var idx = filter.OrderBy.IndexOf(matching);
//        filter.OrderBy.Remove(matching);
//        filter.OrderBy.Insert(idx, item);
//        return true;
//    }

//    public static bool Update(this Filter filter, Paging item)
//    {

//        PagingOutOfRangeException.ThrowIfLessThanZero(item.Skip);
//        PagingOutOfRangeException.ThrowIfLessThanZero(item.Take);
//        filter.Paging = item;
//        return true;
//    }

//    public static bool UpdateCriterion(this Filter filter, string propertyName, object? propertyValue, ComparisonType comparisonType = ComparisonType.Equals, IgnoreCase ignoreCase = IgnoreCase.No)
//    {
//        var item = new Criterion(propertyName, propertyValue, comparisonType, ignoreCase);
//        return filter.Update(item);
//    }

//    public static bool UpdateOrderByProperty(this Filter filter, string propertyName, ListSortDirection sortDirection = ListSortDirection.Ascending)
//    {
//        var item = new OrderByProperty(propertyName, sortDirection);
//        return filter.Update(item);
//    }

//    public static ValidationResult ValidateFilter<T>(this Filter<T> filter) where T : new()
//    {

//        if (filter.Criteria.Count == 0 && filter.OrderBy.Count == 0)
//            return ValidationResult.Success!;

//        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
//            .Select(p => p.Name)
//            .ToHashSet(StringComparer.OrdinalIgnoreCase);

//        var errors = filter.Criteria.Where(c => !properties.Contains(c.PropertyName))
//            .Select(c => new ValidationResult($"Property '{c.PropertyName}' does not exist in type {typeof(T).Name}.", [nameof(Filter.Criteria)]))
//            .Concat(filter.OrderBy.Where(o => !properties.Contains(o.PropertyName))
//                .Select(o => new ValidationResult($"Property '{o.PropertyName}' does not exist in type {typeof(T).Name}.", [nameof(Filter.OrderBy)])))
//            .ToList();

//        return errors.Count > 0
//            ? new ValidationResult($"Filter contains {errors.Count} invalid property references.", errors.SelectMany(e => e.MemberNames))
//            : ValidationResult.Success!;
//    }

//    public static Filter Convert(this Filter<T> source) where T : new()
//    {
//        var target = new Filter
//        {
//            Criteria = source.Criteria,
//            Paging = source.Paging,
//            OrderBy = source.OrderBy
//        };
//        return target;
//    }

//    public static Filter<TDestination> Convert<TSource, TDestination>(this Filter<TSource> source)
//        where TSource : new()
//        where TDestination : new()
//    {
//        var target = new Filter<TDestination>
//        {
//            Criteria = source.Criteria,
//            OrderBy = source.OrderBy,
//            Paging = source.Paging
//        };
//        return target;
//    }

//}