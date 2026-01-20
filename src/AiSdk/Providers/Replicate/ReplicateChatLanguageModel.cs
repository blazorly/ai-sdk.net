using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Replicate.Exceptions;
using AiSdk.Providers.Replicate.Models;

namespace AiSdk.Providers.Replicate;

/// <summary>
/// Replicate implementation of ILanguageModel.
/// </summary>
public class ReplicateChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly ReplicateConfiguration _config;
    private readonly string _modelVersion;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "replicate";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelVersion;

    /// <summary>
    /// Gets the supported URL patterns by media type for this provider.
    /// </summary>
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        // Replicate models generally don't support URL-based content in the same way
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicateChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelVersion">The Replicate model version (e.g., "meta/llama-2-70b-chat").</param>
    /// <param name="config">The Replicate configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public ReplicateChatLanguageModel(
        string modelVersion,
        ReplicateConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelVersion);
        ArgumentNullException.ThrowIfNull(config);

        _modelVersion = modelVersion;
        _config = config;
        _httpClient = httpClient ?? new HttpClient();

        ConfigureHttpClient();
    }

    /// <summary>
    /// Generates text from the language model (non-streaming).
    /// </summary>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var request = BuildRequest(options, stream: false);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("predictions", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<ReplicateResponse>(cancellationToken)
            ?? throw new ReplicateException("Failed to deserialize Replicate response");

        // Wait for prediction to complete
        var completedResult = await WaitForCompletion(result.Id, cancellationToken);

        return MapToGenerateResult(completedResult);
    }

    /// <summary>
    /// Streams text generation from the language model.
    /// </summary>
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var request = BuildRequest(options, stream: true);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "predictions")
        {
            Content = content
        };
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("event: output"))
            {
                // Next line should be the data
                continue;
            }

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);

                // Skip empty data
                if (string.IsNullOrWhiteSpace(data))
                    continue;

                // Try to parse as text chunk
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.TextDelta,
                    Delta = data
                };
            }
            else if (line.StartsWith("event: done"))
            {
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.Finish,
                    FinishReason = FinishReason.Stop
                };
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _config.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "AiSdk.NET/1.0");

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private ReplicateRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var systemPrompt = options.Messages
            .Where(m => m.Role == MessageRole.System)
            .Select(m => m.Content)
            .FirstOrDefault();

        var conversationMessages = options.Messages
            .Where(m => m.Role != MessageRole.System)
            .Select(m => $"{MapRole(m.Role)}: {m.Content}")
            .ToList();

        var prompt = string.Join("\n", conversationMessages);
        if (!string.IsNullOrEmpty(prompt))
        {
            prompt += $"\n{MapRole(MessageRole.Assistant)}:";
        }

        var stopSequences = options.StopSequences != null && options.StopSequences.Count > 0
            ? string.Join(",", options.StopSequences)
            : null;

        var input = new ReplicateInput
        {
            Prompt = prompt,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            StopSequences = stopSequences,
            SystemPrompt = systemPrompt
        };

        return new ReplicateRequest
        {
            Version = _modelVersion,
            Input = input,
            Stream = stream ? true : null
        };
    }

    private async Task<ReplicateResponse> WaitForCompletion(string predictionId, CancellationToken cancellationToken)
    {
        var maxAttempts = 60; // 60 attempts with 1 second delay = 60 seconds max
        var delayMs = 1000;

        for (int i = 0; i < maxAttempts; i++)
        {
            var response = await _httpClient.GetAsync($"predictions/{predictionId}", cancellationToken);
            await EnsureSuccessStatusCode(response);

            var result = await response.Content.ReadFromJsonAsync<ReplicateResponse>(cancellationToken)
                ?? throw new ReplicateException("Failed to deserialize Replicate response");

            if (result.Status == "succeeded" || result.Status == "failed" || result.Status == "canceled")
            {
                if (result.Status == "failed")
                {
                    throw new ReplicateException(result.Error ?? "Prediction failed");
                }

                if (result.Status == "canceled")
                {
                    throw new ReplicateException("Prediction was canceled");
                }

                return result;
            }

            await Task.Delay(delayMs, cancellationToken);
        }

        throw new ReplicateException("Prediction timed out waiting for completion");
    }

    private static LanguageModelGenerateResult MapToGenerateResult(ReplicateResponse response)
    {
        string? text = null;

        if (response.Output != null)
        {
            // Output can be a string or array of strings
            if (response.Output is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    text = jsonElement.GetString();
                }
                else if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    var parts = new List<string>();
                    foreach (var element in jsonElement.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.String)
                        {
                            parts.Add(element.GetString() ?? "");
                        }
                    }
                    text = string.Join("", parts);
                }
            }
            else
            {
                text = response.Output.ToString();
            }
        }

        return new LanguageModelGenerateResult
        {
            Text = text,
            FinishReason = response.Status == "succeeded" ? FinishReason.Stop : FinishReason.Other,
            Usage = new Usage() // Replicate doesn't provide token usage in the standard way
        };
    }

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.System => "System",
        MessageRole.User => "User",
        MessageRole.Assistant => "Assistant",
        MessageRole.Tool => "Tool",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<ReplicateErrorResponse>(content);
                throw new ReplicateException(
                    errorResponse?.Detail ?? errorResponse?.Title ?? "Replicate API error",
                    (int)response.StatusCode,
                    errorResponse?.Status?.ToString());
            }
            catch (JsonException)
            {
                throw new ReplicateException($"Replicate API error: {content}", (int)response.StatusCode, null);
            }
        }
    }

    private static async IAsyncEnumerable<string> ReadLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line != null)
            {
                yield return line;
            }
        }
    }
}
