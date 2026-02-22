using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI.Exceptions;
using AiSdk.Providers.OpenAI.Models;

namespace AiSdk.Providers.OpenAI;

/// <summary>
/// OpenAI implementation of ITranscriptionModel.
/// Uses the Whisper model for audio transcription.
/// </summary>
public class OpenAITranscriptionModel : ITranscriptionModel
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
    /// Initializes a new instance of the <see cref="OpenAITranscriptionModel"/> class.
    /// </summary>
    /// <param name="modelId">The OpenAI transcription model ID (e.g., "whisper-1").</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public OpenAITranscriptionModel(
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
    public async Task<TranscriptionResult> TranscribeAsync(
        byte[] audioData,
        TranscriptionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audioData);

        var fileFormat = options?.FileFormat ?? "wav";
        var fileName = $"audio.{fileFormat}";

        using var formContent = new MultipartFormDataContent();
        formContent.Add(new ByteArrayContent(audioData), "file", fileName);
        formContent.Add(new StringContent(_modelId), "model");
        formContent.Add(new StringContent("verbose_json"), "response_format");

        if (options?.Language != null)
        {
            formContent.Add(new StringContent(options.Language), "language");
        }

        if (options?.Prompt != null)
        {
            formContent.Add(new StringContent(options.Prompt), "prompt");
        }

        if (options?.Temperature.HasValue == true)
        {
            formContent.Add(new StringContent(options.Temperature.Value.ToString()), "temperature");
        }

        var response = await _httpClient.PostAsync("audio/transcriptions", formContent, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<OpenAITranscriptionResponse>(cancellationToken)
            ?? throw new OpenAIException("Failed to deserialize OpenAI transcription response");

        return new TranscriptionResult
        {
            Text = result.Text,
            Language = result.Language,
            Duration = result.Duration
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

internal record OpenAITranscriptionResponse
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    [JsonPropertyName("language")]
    public string? Language { get; init; }

    [JsonPropertyName("duration")]
    public double? Duration { get; init; }
}
