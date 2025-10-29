{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://visionarycoder.framework/queryfilter.schema.json",
  "title": "QueryFilter",
  "description": "Schema for serializing QueryFilter<T> across service boundaries.",
  "type": "object",
  "oneOf": [
    {
      "title": "PropertyFilter",
      "type": "object",
      "properties": {
        "operator": {
          "type": "string",
          "enum": ["Contains", "StartsWith", "EndsWith"]
        },
        "property": { "type": "string" },
        "value": { "type": ["string", "null"] },
        "ignoreCase": { "type": "boolean", "default": false }
      },
      "required": ["operator", "property"]
    },
    {
      "title": "CompositeFilter",
      "type": "object",
      "properties": {
        "operator": {
          "type": "string",
          "enum": ["And", "Or", "Not"]
        },
        "children": {
          "type": "array",
          "items": { "$ref": "#" },
          "minItems": 1
        }
      },
      "required": ["operator", "children"]
    }
  ]
}
