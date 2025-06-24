//using System.Diagnostics;
//using System.Linq.Expressions;

//namespace vc.Ifx.Filtering;

//public class FilteringStrategy<T> 
//{

//    private const string CONTAINS = "Contains";
//    private const string EXPRESSION = "e";
//    private const string TO_STRING = "ToString";
//    private const string TO_LOWER = "ToLower";

//    public IQueryable<T> ApplyFiltering(IQueryable<T> query, Filter filter)
//    {
//        foreach (var criterion in filter.FilterCriteria.Criteria)
//        {
//            if (typeof(T).GetProperty(criterion.PropertyName) == null)
//            {
//                Trace.WriteLine($"PropertyName does not exist in the target type: Type='{typeof(T)}', PropertyName={criterion.PropertyName}");
//                continue;
//            }
//            var predicate = BuildPredicate(criterion);
//            query = query.Where(predicate);
//        }
//        return query;
//    }

//    private Expression<Func<T, bool>> BuildPredicate(FilterCriteria.Criterion criterion)
//    {

//        var parameter = Expression.Parameter(typeof(T), EXPRESSION);
//        var property = Expression.Property(parameter, criterion.PropertyName);
//        var constant = Expression.Constant(criterion.PropertyValue);

//        var body = criterion.ComparisonOperator switch
//        {
//            FilterCriteria.ComparisonType.Equals => Expression.Equal(property, constant),
//            FilterCriteria.ComparisonType.NotEquals => Expression.NotEqual(property, constant),
//            FilterCriteria.ComparisonType.GreaterThan => Expression.GreaterThan(property, constant),
//            FilterCriteria.ComparisonType.LessThan => Expression.LessThan(property, constant),
//            FilterCriteria.ComparisonType.Contains when criterion.IgnoreCase ==  FilterCriteria.IgnoreCase.Yes => BuildCaseInsensitiveContains(property, constant),
//            FilterCriteria.ComparisonType.Contains => Expression.Call(property, CONTAINS, null, constant),
//            var _ => throw new NotSupportedException($"ComparisonOperator {criterion.ComparisonOperator} is not supported.")
//        };
//        return Expression.Lambda<Func<T, bool>>(body, parameter);
//    }

//    private static Expression BuildCaseInsensitiveContains(Expression property, Expression constant)
//    {
//        var toStringCall = Expression.Call(property, TO_STRING, null);
//        var toLowerCall = Expression.Call(toStringCall, TO_LOWER, null);
//        var valueToLower = Expression.Call(constant, TO_STRING, null);
//        var constantToLower = Expression.Call(valueToLower, TO_LOWER, null);
//        return Expression.Call(toLowerCall, CONTAINS, null, constantToLower);
//    }

//}
