using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.HuggingFace.Exceptions;
using AiSdk.Providers.HuggingFace.Models;

namespace AiSdk.Providers.HuggingFace;

/// <summary>
/// Hugging Face implementation of ILanguageModel.
/// </summary>
public class HuggingFaceChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly HuggingFaceConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "huggingface";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Gets the supported URL patterns by media type for this provider.
    /// </summary>
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        // Hugging Face Inference API does not support URL-based inputs natively
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Hugging Face model ID (e.g., "meta-llama/Llama-2-70b-chat-hf").</param>
    /// <param name="config">The Hugging Face configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public HuggingFaceChatLanguageModel(
        string modelId,
        HuggingFaceConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        _modelId = modelId;
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

        var response = await _httpClient.PostAsync(_modelId, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // The response is an array with a single object
        var results = JsonSerializer.Deserialize<List<HuggingFaceResponse>>(responseJson);

        if (results == null || results.Count == 0)
        {
            throw new HuggingFaceException("Failed to deserialize Hugging Face response");
        }

        var result = results[0];

        if (!string.IsNullOrEmpty(result.Error))
        {
            throw new HuggingFaceException(result.Error);
        }

        return MapToGenerateResult(result);
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

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _modelId)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Hugging Face streams JSON objects line by line
            var chunk = JsonSerializer.Deserialize<HuggingFaceStreamResponse>(line);

            if (chunk?.Token?.Text != null && !chunk.Token.Special)
            {
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.TextDelta,
                    Delta = chunk.Token.Text
                };
            }

            if (chunk?.Details?.FinishReason != null)
            {
                var finishReason = MapFinishReason(chunk.Details.FinishReason);
                var usage = chunk.Details.GeneratedTokens.HasValue
                    ? new Usage(InputTokens: 0, OutputTokens: chunk.Details.GeneratedTokens.Value, TotalTokens: chunk.Details.GeneratedTokens.Value)
                    : null;

                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.Finish,
                    FinishReason = finishReason,
                    Usage = usage
                };
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private HuggingFaceRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        // Convert messages to a single input string
        var inputBuilder = new StringBuilder();

        foreach (var message in options.Messages)
        {
            var role = MapRole(message.Role);

            // Format messages in a chat template style
            if (message.Role == MessageRole.System)
            {
                inputBuilder.AppendLine($"<<SYS>>{message.Content}<</SYS>>");
            }
            else if (message.Role == MessageRole.User)
            {
                inputBuilder.AppendLine($"[INST] {message.Content} [/INST]");
            }
            else if (message.Role == MessageRole.Assistant)
            {
                inputBuilder.AppendLine(message.Content);
            }
        }

        var parameters = new HuggingFaceParameters
        {
            MaxNewTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stop = options.StopSequences,
            ReturnFullText = false
        };

        return new HuggingFaceRequest
        {
            Inputs = inputBuilder.ToString().Trim(),
            Parameters = parameters,
            Stream = stream
        };
    }

    private static LanguageModelGenerateResult MapToGenerateResult(HuggingFaceResponse response)
    {
        return new LanguageModelGenerateResult
        {
            Text = response.GeneratedText,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(InputTokens: 0, OutputTokens: 0, TotalTokens: 0)
        };
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "length" => FinishReason.Length,
        "eos_token" => FinishReason.Stop,
        "stop_sequence" => FinishReason.Stop,
        _ => FinishReason.Other
    };

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.System => "system",
        MessageRole.User => "user",
        MessageRole.Assistant => "assistant",
        MessageRole.Tool => "tool",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<HuggingFaceErrorResponse>(content);
                throw new HuggingFaceException(
                    errorResponse?.Error ?? "Hugging Face API error",
                    (int)response.StatusCode,
                    errorResponse?.ErrorType);
            }
            catch (JsonException)
            {
                throw new HuggingFaceException($"Hugging Face API error: {content}", (int)response.StatusCode, null);
            }
        }
    }

    private static async IAsyncEnumerable<string> ReadLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            yield return line;
        }
    }
}
