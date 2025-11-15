using System.Text.Json;

namespace VisionaryCoder.Framework.Querying.Serialization;

/// <summary>
/// Validates QueryFilter JSON against the JSON Schema.
/// </summary>
public static class QueryFilterSchemaValidator
{
    private static readonly Lazy<JsonSchema> schema = new(LoadSchema);

    private static JsonSchema LoadSchema()
    {
        string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".json", "schemas", "queryfilter.schema.json");
        string schemaJson = File.ReadAllText(schemaPath);
        return JsonSchema.FromJsonAsync(schemaJson).Result;
    }

    /// <summary>
    /// Validates a QueryFilter JSON string against the schema.
    /// </summary>
    /// <param name="json">The JSON string to validate.</param>
    /// <returns>A list of validation errors, or empty if valid.</returns>
    public static IReadOnlyList<string> Validate(string json)
    {
        ICollection<ValidationError> errors = schema.Value.Validate(json);
        return errors.Select(e => e.ToString()).ToList();
    }

    /// <summary>
    /// Validates a QueryFilter JSON document against the schema.
    /// </summary>
    /// <param name="jsonDocument">The JSON document to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool IsValid(JsonDocument jsonDocument)
    {
        string json = jsonDocument.RootElement.GetRawText();
        return schema.Value.Validate(json).Count == 0;
    }
}
