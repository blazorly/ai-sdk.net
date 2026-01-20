using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiSdk.Utilities;

/// <summary>
/// Generates JSON schemas from .NET types for use with structured output and tool definitions.
/// </summary>
internal static class JsonSchemaGenerator
{
    /// <summary>
    /// Generates a JSON schema for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to generate a schema for.</typeparam>
    /// <returns>A JsonDocument containing the JSON schema.</returns>
    public static JsonDocument GenerateSchema<T>()
    {
        return GenerateSchema(typeof(T));
    }

    /// <summary>
    /// Generates a JSON schema for the specified type.
    /// </summary>
    /// <param name="type">The type to generate a schema for.</param>
    /// <returns>A JsonDocument containing the JSON schema.</returns>
    public static JsonDocument GenerateSchema(Type type)
    {
        var schema = new Dictionary<string, object>();
        BuildSchema(type, schema);

        var json = JsonSerializer.Serialize(schema, new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        return JsonDocument.Parse(json);
    }

    private static void BuildSchema(Type type, Dictionary<string, object> schema)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(string))
        {
            schema["type"] = "string";
        }
        else if (underlyingType == typeof(int) || underlyingType == typeof(long) ||
                 underlyingType == typeof(short) || underlyingType == typeof(byte))
        {
            schema["type"] = "integer";
        }
        else if (underlyingType == typeof(float) || underlyingType == typeof(double) ||
                 underlyingType == typeof(decimal))
        {
            schema["type"] = "number";
        }
        else if (underlyingType == typeof(bool))
        {
            schema["type"] = "boolean";
        }
        else if (underlyingType.IsEnum)
        {
            schema["type"] = "string";
            schema["enum"] = Enum.GetNames(underlyingType);
        }
        else if (underlyingType.IsArray || (underlyingType.IsGenericType &&
                 underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            schema["type"] = "array";
            var elementType = underlyingType.IsArray
                ? underlyingType.GetElementType()!
                : underlyingType.GetGenericArguments()[0];

            var items = new Dictionary<string, object>();
            BuildSchema(elementType, items);
            schema["items"] = items;
        }
        else if (underlyingType.IsClass || underlyingType.IsValueType)
        {
            schema["type"] = "object";
            var properties = new Dictionary<string, object>();
            var required = new List<string>();

            var props = underlyingType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var propName = GetJsonPropertyName(prop);
                var propSchema = new Dictionary<string, object>();
                BuildSchema(prop.PropertyType, propSchema);

                var description = GetPropertyDescription(prop);
                if (!string.IsNullOrEmpty(description))
                {
                    propSchema["description"] = description;
                }

                properties[propName] = propSchema;

                if (!IsNullable(prop.PropertyType))
                {
                    required.Add(propName);
                }
            }

            schema["properties"] = properties;
            if (required.Count > 0)
            {
                schema["required"] = required;
            }
        }
    }

    private static string GetJsonPropertyName(PropertyInfo property)
    {
        var jsonPropertyAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        if (jsonPropertyAttr != null)
        {
            return jsonPropertyAttr.Name;
        }

        var name = property.Name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    private static string? GetPropertyDescription(PropertyInfo property)
    {
        var descAttr = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
        return descAttr?.Description;
    }

    private static bool IsNullable(Type type)
    {
        if (!type.IsValueType)
            return true;

        return Nullable.GetUnderlyingType(type) != null;
    }
}
