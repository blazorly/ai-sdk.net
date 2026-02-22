using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Google.Exceptions;
using AiSdk.Providers.Google.Models;

namespace AiSdk.Providers.Google;

/// <summary>
/// Google Gemini implementation of IEmbeddingModel.
/// Supports text-embedding-004 and other Google embedding models.
/// </summary>
public class GoogleEmbeddingModel : IEmbeddingModel
{
    private readonly HttpClient _httpClient;
    private readonly GoogleConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "google";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleEmbeddingModel"/> class.
    /// </summary>
    /// <param name="modelId">The Google embedding model ID (e.g., "text-embedding-004").</param>
    /// <param name="config">The Google configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public GoogleEmbeddingModel(
        string modelId,
        GoogleConfiguration config,
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

        var request = new GoogleEmbeddingRequest
        {
            Model = $"models/{_modelId}",
            Content = new GoogleEmbeddingContent
            {
                Parts = new List<GoogleEmbeddingPart>
                {
                    new() { Text = input }
                }
            },
            OutputDimensionality = options?.Dimensions
        };

        var url = $"models/{_modelId}:embedContent?key={_config.ApiKey}";
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<GoogleEmbeddingResponse>(cancellationToken)
            ?? throw new GoogleException("Failed to deserialize Google embedding response");

        return new EmbeddingResult
        {
            Embedding = result.Embedding.Values,
            Usage = new Usage(InputTokens: null, TotalTokens: null)
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

        var batchRequest = new GoogleBatchEmbeddingRequest
        {
            Requests = inputList.Select(text => new GoogleEmbeddingRequest
            {
                Model = $"models/{_modelId}",
                Content = new GoogleEmbeddingContent
                {
                    Parts = new List<GoogleEmbeddingPart>
                    {
                        new() { Text = text }
                    }
                },
                OutputDimensionality = options?.Dimensions
            }).ToList()
        };

        var url = $"models/{_modelId}:batchEmbedContents?key={_config.ApiKey}";
        var json = JsonSerializer.Serialize(batchRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<GoogleBatchEmbeddingResponse>(cancellationToken)
            ?? throw new GoogleException("Failed to deserialize Google batch embedding response");

        var embeddings = result.Embeddings
            .Select(e => e.Values)
            .ToList()
            .AsReadOnly();

        return new BatchEmbeddingResult
        {
            Embeddings = embeddings,
            Usage = new Usage(InputTokens: null, TotalTokens: null)
        };
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);

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
            throw new GoogleException(
                $"Google API error: {content}",
                (int)response.StatusCode,
                null);
        }
    }
}
