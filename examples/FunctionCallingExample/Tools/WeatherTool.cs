using AiSdk.Tools;

namespace FunctionCallingExample.Tools;

/// <summary>
/// Input parameters for the weather tool.
/// </summary>
public record WeatherInput
{
    /// <summary>
    /// The city to get weather for.
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Temperature unit (celsius or fahrenheit).
    /// </summary>
    public string Unit { get; init; } = "celsius";
}

/// <summary>
/// Output data from the weather tool.
/// </summary>
public record WeatherOutput
{
    public required string City { get; init; }
    public required double Temperature { get; init; }
    public required string Unit { get; init; }
    public required string Condition { get; init; }
    public required int Humidity { get; init; }
}

/// <summary>
/// A tool that provides weather information for cities.
/// In production, this would call a real weather API.
/// </summary>
public static class WeatherTool
{
    /// <summary>
    /// Creates the weather tool definition.
    /// </summary>
    public static ToolWithExecution<WeatherInput, WeatherOutput> Create()
    {
        return Tool.Create<WeatherInput, WeatherOutput>(
            name: "get_weather",
            description: "Get the current weather for a specified city. Returns temperature, conditions, and humidity.",
            execute: GetWeather
        );
    }

    /// <summary>
    /// Simulates getting weather data for a city.
    /// In production, this would make an API call to a weather service.
    /// </summary>
    private static WeatherOutput GetWeather(WeatherInput input)
    {
        // Simulate weather data based on city name
        // In production, you would call a real weather API like OpenWeatherMap
        var mockWeatherData = new Dictionary<string, (double temp, string condition, int humidity)>
        {
            { "london", (15.0, "Cloudy", 75) },
            { "paris", (18.0, "Sunny", 60) },
            { "new york", (22.0, "Partly Cloudy", 65) },
            { "tokyo", (20.0, "Rainy", 80) },
            { "sydney", (25.0, "Sunny", 55) },
            { "san francisco", (16.0, "Foggy", 70) },
            { "berlin", (12.0, "Overcast", 72) },
            { "mumbai", (32.0, "Hot and Humid", 85) }
        };

        var cityLower = input.City.ToLowerInvariant();
        var (tempCelsius, condition, humidity) = mockWeatherData.ContainsKey(cityLower)
            ? mockWeatherData[cityLower]
            : (20.0, "Clear", 50); // Default weather

        // Convert to Fahrenheit if requested
        var temperature = input.Unit.ToLowerInvariant() == "fahrenheit"
            ? (tempCelsius * 9 / 5) + 32
            : tempCelsius;

        return new WeatherOutput
        {
            City = input.City,
            Temperature = Math.Round(temperature, 1),
            Unit = input.Unit,
            Condition = condition,
            Humidity = humidity
        };
    }
}
