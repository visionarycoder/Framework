//            amespace Wsdot.Idl.Ifx.Filtering.v3;

//public class CriterionBuilder
//{
//    private string propertyNameImp = string.Empty;
//    private object? propertyValueImp;
//    private ComparisonType comparisonTypeImp = ComparisonType.Equals;
//    private StringComparison stringComparisonImp = StringComparison.CurrentCulture;

//    public CriterionBuilder ForProperty(string propertyName)
//    {
//        propertyNameImp = propertyName;
//        return this;
//    }

//    public CriterionBuilder WithValue(object? value)
//    {
//        propertyValueImp = value;
//        return this;
//    }

//    public CriterionBuilder UsingComparison(ComparisonType comparisonType)
//    {
//        comparisonTypeImp = comparisonType;
//        return this;
//    }

//    public CriterionBuilder WithStringComparison(StringComparison stringComparison)
//    {
//        stringComparisonImp = stringComparison;
//        return this;
//    }

//    // Convenience helpers
//    public CriterionBuilder RespectCaseSensitivity()
//    {
//        stringComparisonImp = StringComparison.CurrentCulture;
//        return this;
//    }

//    public CriterionBuilder IgnoreCaseSensitivity()
//    {
//        stringComparisonImp = StringComparison.CurrentCultureIgnoreCase;
//        return this;
//    }

//    public Criterion Build()
//    {
//        return new Criterion(propertyNameImp, propertyValueImp, comparisonTypeImp, stringComparisonImp);
//    }
//}