using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI.Exceptions;
using AiSdk.Providers.OpenAI.Models;

namespace AiSdk.Providers.OpenAI;

/// <summary>
/// OpenAI implementation of IImageGenerationModel.
/// Supports DALL-E 2 and DALL-E 3.
/// </summary>
public class OpenAIImageGenerationModel : IImageGenerationModel
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "openai";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIImageGenerationModel"/> class.
    /// </summary>
    /// <param name="modelId">The OpenAI image model ID (e.g., "dall-e-3").</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public OpenAIImageGenerationModel(
        string modelId,
        OpenAIConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        _modelId = modelId;
        _config = config;
        _httpClient = httpClient ?? new HttpClient();

        ConfigureHttpClient();
    }

    /// <inheritdoc/>
    public async Task<ImageGenerationResult> GenerateImageAsync(
        ImageGenerationOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var request = new OpenAIImageRequest
        {
            Model = _modelId,
            Prompt = options.Prompt,
            N = options.N ?? 1,
            Size = options.Size ?? "1024x1024",
            Quality = options.Quality,
            Style = options.Style,
            ResponseFormat = "b64_json"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("images/generations", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<OpenAIImageResponse>(cancellationToken)
            ?? throw new OpenAIException("Failed to deserialize OpenAI image response");

        var images = result.Data.Select(d => new GeneratedImage
        {
            Data = d.B64Json != null ? Convert.FromBase64String(d.B64Json) : null,
            Url = d.Url,
            RevisedPrompt = d.RevisedPrompt
        }).ToList().AsReadOnly();

        return new ImageGenerationResult
        {
            Images = images
        };
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);

        if (!string.IsNullOrEmpty(_config.Organization))
        {
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", _config.Organization);
        }

        if (!string.IsNullOrEmpty(_config.Project))
        {
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Project", _config.Project);
        }

        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<OpenAIErrorResponse>(content);
                throw new OpenAIException(
                    errorResponse?.Error.Message ?? "OpenAI API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new OpenAIException($"OpenAI API error: {content}", (int)response.StatusCode, null);
            }
        }
    }
}

internal record OpenAIImageRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("n")]
    public int N { get; init; } = 1;

    [JsonPropertyName("size")]
    public string Size { get; init; } = "1024x1024";

    [JsonPropertyName("quality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Quality { get; init; }

    [JsonPropertyName("style")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Style { get; init; }

    [JsonPropertyName("response_format")]
    public string ResponseFormat { get; init; } = "b64_json";
}

internal record OpenAIImageResponse
{
    [JsonPropertyName("created")]
    public long Created { get; init; }

    [JsonPropertyName("data")]
    public required List<OpenAIImageData> Data { get; init; }
}

internal record OpenAIImageData
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("b64_json")]
    public string? B64Json { get; init; }

    [JsonPropertyName("revised_prompt")]
    public string? RevisedPrompt { get; init; }
}
