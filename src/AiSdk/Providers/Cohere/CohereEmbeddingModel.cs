using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Cohere.Exceptions;
using AiSdk.Providers.Cohere.Models;

namespace AiSdk.Providers.Cohere;

/// <summary>
/// Cohere implementation of IEmbeddingModel.
/// Supports embed-v3 and other Cohere embedding models.
/// </summary>
public class CohereEmbeddingModel : IEmbeddingModel
{
    private readonly HttpClient _httpClient;
    private readonly CohereConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "cohere";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereEmbeddingModel"/> class.
    /// </summary>
    /// <param name="modelId">The Cohere embedding model ID (e.g., "embed-english-v3.0").</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CohereEmbeddingModel(
        string modelId,
        CohereConfiguration config,
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

        var inputType = GetInputType(options);

        var request = new CohereEmbeddingRequest
        {
            Texts = new List<string> { input },
            Model = _modelId,
            InputType = inputType,
            EmbeddingTypes = new List<string> { "float" }
        };

        var response = await SendEmbeddingRequest(request, cancellationToken);

        var embedding = response.Embeddings.Float?[0]
            ?? throw new CohereException("No float embeddings in response");

        return new EmbeddingResult
        {
            Embedding = embedding,
            Usage = new Usage(
                InputTokens: response.Meta?.BilledUnits?.InputTokens,
                TotalTokens: response.Meta?.BilledUnits?.InputTokens)
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

        var inputType = GetInputType(options);

        var request = new CohereEmbeddingRequest
        {
            Texts = inputList,
            Model = _modelId,
            InputType = inputType,
            EmbeddingTypes = new List<string> { "float" }
        };

        var response = await SendEmbeddingRequest(request, cancellationToken);

        var embeddings = response.Embeddings.Float
            ?? throw new CohereException("No float embeddings in response");

        return new BatchEmbeddingResult
        {
            Embeddings = embeddings.AsReadOnly(),
            Usage = new Usage(
                InputTokens: response.Meta?.BilledUnits?.InputTokens,
                TotalTokens: response.Meta?.BilledUnits?.InputTokens)
        };
    }

    private static string GetInputType(EmbeddingOptions? options)
    {
        if (options?.ProviderOptions != null &&
            options.ProviderOptions.TryGetValue("input_type", out var inputType))
        {
            return inputType.ToString() ?? "search_document";
        }

        return "search_document";
    }

    private async Task<CohereEmbeddingResponse> SendEmbeddingRequest(
        CohereEmbeddingRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("embed", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        return await response.Content.ReadFromJsonAsync<CohereEmbeddingResponse>(cancellationToken)
            ?? throw new CohereException("Failed to deserialize Cohere embedding response");
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);

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
            throw new CohereException(
                $"Cohere API error: {content}",
                (int)response.StatusCode);
        }
    }
}
