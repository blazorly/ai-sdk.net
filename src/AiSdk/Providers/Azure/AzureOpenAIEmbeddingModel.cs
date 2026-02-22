using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Azure.Exceptions;
using AiSdk.Providers.Azure.Models;

namespace AiSdk.Providers.Azure;

/// <summary>
/// Azure OpenAI implementation of IEmbeddingModel.
/// Uses the same API format as OpenAI but deployed through Azure.
/// </summary>
public class AzureOpenAIEmbeddingModel : IEmbeddingModel
{
    private readonly HttpClient _httpClient;
    private readonly AzureOpenAIConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "azure-openai";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIEmbeddingModel"/> class.
    /// </summary>
    /// <param name="modelId">The model identifier for display purposes.</param>
    /// <param name="config">The Azure OpenAI configuration (DeploymentName should be the embedding deployment).</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public AzureOpenAIEmbeddingModel(
        string modelId,
        AzureOpenAIConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

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

        var request = new AzureOpenAIEmbeddingRequest
        {
            Input = input,
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

        var request = new AzureOpenAIEmbeddingRequest
        {
            Input = inputList,
            Dimensions = options?.Dimensions,
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

    private async Task<AzureOpenAIEmbeddingResponse> SendEmbeddingRequest(
        AzureOpenAIEmbeddingRequest request,
        CancellationToken cancellationToken)
    {
        var url = BuildEndpoint("embeddings");
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        return await response.Content.ReadFromJsonAsync<AzureOpenAIEmbeddingResponse>(cancellationToken)
            ?? throw new AzureOpenAIException("Failed to deserialize Azure OpenAI embedding response");
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrWhiteSpace(_config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Remove("api-key");
            _httpClient.DefaultRequestHeaders.Add("api-key", _config.ApiKey);
        }
        else if (!string.IsNullOrWhiteSpace(_config.AzureAdToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.AzureAdToken);
        }

        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private string BuildEndpoint(string path)
    {
        var endpoint = _config.Endpoint.TrimEnd('/');
        var deploymentName = _config.DeploymentName;
        var apiVersion = _config.ApiVersion;

        return $"{endpoint}/openai/deployments/{deploymentName}/{path}?api-version={apiVersion}";
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<AzureOpenAIErrorResponse>(content);
                throw new AzureOpenAIException(
                    errorResponse?.Error.Message ?? "Azure OpenAI API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new AzureOpenAIException($"Azure OpenAI API error: {content}", (int)response.StatusCode, null);
            }
        }
    }
}
