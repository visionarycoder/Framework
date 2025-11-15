using System.Text.Json;

namespace VisionaryCoder.Framework.AppConfiguration;

internal static class AppConfigurationHelper
{

    public static T ConvertValue<T>(string stringValue, T defaultValue)
    {
        try
        {
            if (typeof(T) == typeof(string))
                return (T)(object)stringValue;

            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(stringValue);

            if (typeof(T) == typeof(long))
                return (T)(object)long.Parse(stringValue);

            if (typeof(T) == typeof(double))
                return (T)(object)double.Parse(stringValue);

            if (typeof(T) == typeof(decimal))
                return (T)(object)decimal.Parse(stringValue);

            if (typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(stringValue);

            if (typeof(T) == typeof(DateTime))
                return (T)(object)DateTime.Parse(stringValue);

            if (typeof(T) == typeof(DateTimeOffset))
                return (T)(object)DateTimeOffset.Parse(stringValue);

            if (typeof(T) == typeof(TimeSpan))
                return (T)(object)TimeSpan.Parse(stringValue);

            if (typeof(T) == typeof(Guid))
                return (T)(object)Guid.Parse(stringValue);

            // For complex types, try JSON deserialization
            return JsonSerializer.Deserialize<T>(stringValue) ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}
