using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiSdk.Abstractions;
using AiSdk.Providers.Cohere.Exceptions;

namespace AiSdk.Providers.Cohere;

/// <summary>
/// Cohere implementation of IRerankModel.
/// Supports rerank-v3.5 and other Cohere reranking models.
/// </summary>
public class CohereRerankModel : IRerankModel
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
    /// Initializes a new instance of the <see cref="CohereRerankModel"/> class.
    /// </summary>
    /// <param name="modelId">The Cohere rerank model ID (e.g., "rerank-v3.5").</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CohereRerankModel(
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
    public async Task<RerankResult> RerankAsync(
        string query,
        IEnumerable<string> documents,
        RerankOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(documents);

        var docList = documents.ToList();
        if (docList.Count == 0)
        {
            return new RerankResult
            {
                Results = Array.Empty<RankedDocument>()
            };
        }

        var request = new CohereRerankRequest
        {
            Model = _modelId,
            Query = query,
            Documents = docList,
            TopN = options?.TopN,
            ReturnDocuments = options?.ReturnDocuments ?? true
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("rerank", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<CohereRerankResponse>(cancellationToken)
            ?? throw new CohereException("Failed to deserialize Cohere rerank response");

        var rankedDocs = result.Results.Select(r => new RankedDocument
        {
            Index = r.Index,
            RelevanceScore = r.RelevanceScore,
            Document = r.Document?.Text
        }).ToList().AsReadOnly();

        return new RerankResult
        {
            Results = rankedDocs,
            Usage = result.Meta?.BilledUnits != null
                ? new Usage(InputTokens: null, TotalTokens: null)
                : null
        };
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

internal record CohereRerankRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }

    [JsonPropertyName("documents")]
    public required List<string> Documents { get; init; }

    [JsonPropertyName("top_n")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TopN { get; init; }

    [JsonPropertyName("return_documents")]
    public bool ReturnDocuments { get; init; } = true;
}

internal record CohereRerankResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("results")]
    public required List<CohereRerankResult> Results { get; init; }

    [JsonPropertyName("meta")]
    public CohereRerankMeta? Meta { get; init; }
}

internal record CohereRerankResult
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("relevance_score")]
    public double RelevanceScore { get; init; }

    [JsonPropertyName("document")]
    public CohereRerankDocument? Document { get; init; }
}

internal record CohereRerankDocument
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

internal record CohereRerankMeta
{
    [JsonPropertyName("billed_units")]
    public CohereRerankBilledUnits? BilledUnits { get; init; }
}

internal record CohereRerankBilledUnits
{
    [JsonPropertyName("search_units")]
    public int? SearchUnits { get; init; }
}
