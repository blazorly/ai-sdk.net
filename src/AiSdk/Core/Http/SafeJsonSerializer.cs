using System.Text.Json;

namespace AiSdk.Core.Http;

/// <summary>
/// Provides safe JSON serialization/deserialization with validation.
/// Never use System.Text.Json directly to prevent security vulnerabilities.
/// </summary>
public static class SafeJsonSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Safely deserializes JSON with validation.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string.</param>
    /// <param name="options">Optional serializer options.</param>
    /// <returns>The deserialized object.</returns>
    public static T Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
            if (result is null)
            {
                throw new JsonException("Deserialization resulted in null value.");
            }
            return result;
        }
        catch (JsonException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new JsonException($"Failed to deserialize JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Safely serializes an object to JSON.
    /// </summary>
    /// <typeparam name="T">The type to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">Optional serializer options.</param>
    /// <returns>The JSON string.</returns>
    public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(value, options ?? DefaultOptions);
    }
}
