using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI.Exceptions;
using AiSdk.Providers.OpenAI.Models;

namespace AiSdk.Providers.OpenAI;

/// <summary>
/// OpenAI implementation of ISpeechModel (text-to-speech).
/// Supports tts-1 and tts-1-hd models.
/// </summary>
public class OpenAISpeechModel : ISpeechModel
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
    /// Initializes a new instance of the <see cref="OpenAISpeechModel"/> class.
    /// </summary>
    /// <param name="modelId">The OpenAI TTS model ID (e.g., "tts-1", "tts-1-hd").</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public OpenAISpeechModel(
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
    public async Task<SpeechResult> GenerateSpeechAsync(
        string text,
        SpeechOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);

        var format = options?.Format ?? "mp3";
        var voice = options?.Voice ?? "alloy";

        var request = new OpenAISpeechRequest
        {
            Model = _modelId,
            Input = text,
            Voice = voice,
            ResponseFormat = format,
            Speed = options?.Speed
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("audio/speech", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var audioData = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var contentType = format switch
        {
            "mp3" => "audio/mpeg",
            "opus" => "audio/opus",
            "aac" => "audio/aac",
            "flac" => "audio/flac",
            "wav" => "audio/wav",
            "pcm" => "audio/pcm",
            _ => "audio/mpeg"
        };

        return new SpeechResult
        {
            AudioData = audioData,
            ContentType = contentType
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

internal record OpenAISpeechRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("voice")]
    public required string Voice { get; init; }

    [JsonPropertyName("response_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResponseFormat { get; init; }

    [JsonPropertyName("speed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Speed { get; init; }
}
