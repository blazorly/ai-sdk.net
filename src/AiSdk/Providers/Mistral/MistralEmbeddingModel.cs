using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Mistral.Exceptions;
using AiSdk.Providers.Mistral.Models;

namespace AiSdk.Providers.Mistral;

/// <summary>
/// Mistral AI implementation of IEmbeddingModel.
/// Supports mistral-embed and other Mistral embedding models.
/// </summary>
public class MistralEmbeddingModel : IEmbeddingModel
{
    private readonly HttpClient _httpClient;
    private readonly MistralConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "mistral";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralEmbeddingModel"/> class.
    /// </summary>
    /// <param name="modelId">The Mistral embedding model ID (e.g., "mistral-embed").</param>
    /// <param name="config">The Mistral configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public MistralEmbeddingModel(
        string modelId,
        MistralConfiguration config,
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

        var request = new MistralEmbeddingRequest
        {
            Model = _modelId,
            Input = new List<string> { input },
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

        var request = new MistralEmbeddingRequest
        {
            Model = _modelId,
            Input = inputList,
            EncodingFormat = "float"
        };

        var response = await SendEmbeddingRequest(request, cancellationToken);

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

    private async Task<MistralEmbeddingResponse> SendEmbeddingRequest(
        MistralEmbeddingRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("embeddings", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        return await response.Content.ReadFromJsonAsync<MistralEmbeddingResponse>(cancellationToken)
            ?? throw new MistralException("Failed to deserialize Mistral embedding response");
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

            try
            {
                var errorResponse = JsonSerializer.Deserialize<MistralErrorResponse>(content);
                throw new MistralException(
                    errorResponse?.Error.Message ?? "Mistral API error",
                    (int)response.StatusCode,
                    null);
            }
            catch (JsonException)
            {
                throw new MistralException($"Mistral API error: {content}", (int)response.StatusCode, null);
            }
        }
    }
}
