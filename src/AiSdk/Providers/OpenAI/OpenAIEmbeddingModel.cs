using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI.Exceptions;
using AiSdk.Providers.OpenAI.Models;

namespace AiSdk.Providers.OpenAI;

/// <summary>
/// OpenAI implementation of IEmbeddingModel.
/// Supports text-embedding-3-small, text-embedding-3-large, and text-embedding-ada-002.
/// </summary>
public class OpenAIEmbeddingModel : IEmbeddingModel
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
    /// Initializes a new instance of the <see cref="OpenAIEmbeddingModel"/> class.
    /// </summary>
    /// <param name="modelId">The OpenAI embedding model ID (e.g., "text-embedding-3-small").</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public OpenAIEmbeddingModel(
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
    public async Task<EmbeddingResult> EmbedAsync(
        string input,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var request = new OpenAIEmbeddingRequest
        {
            Input = input,
            Model = _modelId,
            Dimensions = options?.Dimensions,
            EncodingFormat = "float"
        };

        var response = await SendEmbeddingRequest(request, cancellationToken);

        return new EmbeddingResult
        {
            Embedding = response.Data[0].Embedding,
            Usage = new Usage(
                InputTokens: response.Usage.PromptTokens,
                TotalTokens: response.Usage.TotalTokens)
        };
    }

    /// <inheritdoc/>
    public async Task<BatchEmbeddingResult> EmbedManyAsync(
        IEnumerable<string> inputs,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inputs);

        var inputList = inputs.ToList();
        if (inputList.Count == 0)
        {
            return new BatchEmbeddingResult
            {
                Embeddings = Array.Empty<float[]>(),
                Usage = new Usage(InputTokens: 0, TotalTokens: 0)
            };
        }

        var request = new OpenAIEmbeddingRequest
        {
            Input = inputList,
            Model = _modelId,
            Dimensions = options?.Dimensions,
            EncodingFormat = "float"
        };

        var response = await SendEmbeddingRequest(request, cancellationToken);

        // Sort by index to ensure correct ordering
        var embeddings = response.Data
            .OrderBy(d => d.Index)
            .Select(d => d.Embedding)
            .ToList()
            .AsReadOnly();

        return new BatchEmbeddingResult
        {
            Embeddings = embeddings,
            Usage = new Usage(
                InputTokens: response.Usage.PromptTokens,
                TotalTokens: response.Usage.TotalTokens)
        };
    }

    private async Task<OpenAIEmbeddingResponse> SendEmbeddingRequest(
        OpenAIEmbeddingRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("embeddings", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        return await response.Content.ReadFromJsonAsync<OpenAIEmbeddingResponse>(cancellationToken)
            ?? throw new OpenAIException("Failed to deserialize OpenAI embedding response");
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
