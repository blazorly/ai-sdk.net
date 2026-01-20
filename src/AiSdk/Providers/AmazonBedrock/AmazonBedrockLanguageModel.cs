using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock.Adapters;
using AiSdk.Providers.AmazonBedrock.Exceptions;

namespace AiSdk.Providers.AmazonBedrock;

/// <summary>
/// Amazon Bedrock implementation of ILanguageModel.
/// Supports multiple model providers through Bedrock: Anthropic Claude, Amazon Titan, Meta Llama, and more.
/// </summary>
public class AmazonBedrockLanguageModel : ILanguageModel, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private readonly AmazonBedrockConfiguration _config;
    private readonly string _modelId;
    private readonly IBedrockModelAdapter _adapter;
    private bool _disposed;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "bedrock";

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
        // Claude models on Bedrock support image URLs
        // Other models have limited multimodal support
        var supported = new Dictionary<string, IReadOnlyList<string>>();

        if (_modelId.StartsWith("anthropic.claude"))
        {
            supported["image/*"] = new List<string> { ".*" }.AsReadOnly();
        }

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonBedrockLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Bedrock model ID (e.g., "anthropic.claude-3-5-sonnet-20241022-v2:0", "amazon.titan-text-express-v1").</param>
    /// <param name="config">The Amazon Bedrock configuration.</param>
    public AmazonBedrockLanguageModel(string modelId, AmazonBedrockConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        _modelId = modelId;
        _config = config;
        _adapter = CreateAdapter(modelId);

        // Create AWS credentials
        AWSCredentials? credentials = null;
        if (!string.IsNullOrEmpty(config.AccessKeyId) && !string.IsNullOrEmpty(config.SecretAccessKey))
        {
            credentials = string.IsNullOrEmpty(config.SessionToken)
                ? new BasicAWSCredentials(config.AccessKeyId, config.SecretAccessKey)
                : new SessionAWSCredentials(config.AccessKeyId, config.SecretAccessKey, config.SessionToken);
        }

        // Create client configuration
        var clientConfig = new AmazonBedrockRuntimeConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(config.Region)
        };

        if (config.TimeoutSeconds.HasValue)
        {
            clientConfig.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds.Value);
        }

        // Create Bedrock Runtime client
        _client = credentials != null
            ? new AmazonBedrockRuntimeClient(credentials, clientConfig)
            : new AmazonBedrockRuntimeClient(clientConfig);
    }

    /// <summary>
    /// Generates text from the language model (non-streaming).
    /// </summary>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            // Build model-specific request
            var requestBody = _adapter.BuildRequest(options);

            // Create Bedrock request
            var request = new InvokeModelRequest
            {
                ModelId = _modelId,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)),
                ContentType = "application/json",
                Accept = "application/json"
            };

            // Invoke model
            var response = await _client.InvokeModelAsync(request, cancellationToken);

            // Parse response
            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync(cancellationToken);

            return _adapter.ParseResponse(responseBody);
        }
        catch (AmazonBedrockRuntimeException ex)
        {
            throw new AmazonBedrockException(
                $"Bedrock API error: {ex.Message}",
                (int?)ex.StatusCode,
                ex.ErrorCode,
                _modelId);
        }
        catch (AmazonServiceException ex)
        {
            throw new AmazonBedrockException(
                $"AWS service error: {ex.Message}",
                (int)ex.StatusCode,
                ex.ErrorCode,
                _modelId);
        }
        catch (Exception ex) when (ex is not AmazonBedrockException)
        {
            throw new AmazonBedrockException(
                $"Unexpected error invoking Bedrock model: {ex.Message}",
                ex);
        }
    }

    /// <summary>
    /// Streams text generation from the language model.
    /// </summary>
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!_adapter.SupportsStreaming)
        {
            throw new NotSupportedException($"Model {_modelId} does not support streaming. Use GenerateAsync instead.");
        }

        // Build model-specific request
        var requestBody = _adapter.BuildRequest(options);

        // Create Bedrock streaming request
        var request = new InvokeModelWithResponseStreamRequest
        {
            ModelId = _modelId,
            Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)),
            ContentType = "application/json",
            Accept = "application/json"
        };

        InvokeModelWithResponseStreamResponse response;
        try
        {
            // Invoke model with streaming
            response = await _client.InvokeModelWithResponseStreamAsync(request, cancellationToken);
        }
        catch (AmazonBedrockRuntimeException ex)
        {
            throw new AmazonBedrockException(
                $"Bedrock streaming error: {ex.Message}",
                (int?)ex.StatusCode,
                ex.ErrorCode,
                _modelId);
        }
        catch (AmazonServiceException ex)
        {
            throw new AmazonBedrockException(
                $"AWS service error during streaming: {ex.Message}",
                (int)ex.StatusCode,
                ex.ErrorCode,
                _modelId);
        }
        catch (Exception ex) when (ex is not AmazonBedrockException and not NotSupportedException)
        {
            throw new AmazonBedrockException(
                $"Unexpected error during streaming: {ex.Message}",
                ex);
        }

        // Process streaming response
        foreach (var streamEvent in response.Body)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            if (streamEvent is PayloadPart payloadPart)
            {
                var chunkBytes = payloadPart.Bytes.ToArray();
                var chunkJson = Encoding.UTF8.GetString(chunkBytes);

                var chunk = _adapter.ParseStreamChunk(chunkJson);
                if (chunk != null)
                {
                    yield return chunk;
                }
            }
            else if (streamEvent is ModelStreamErrorException errorEvent)
            {
                throw new AmazonBedrockException(
                    $"Streaming error: {errorEvent.Message}",
                    null,
                    null,
                    _modelId);
            }
        }
    }

    /// <summary>
    /// Creates the appropriate adapter based on the model ID prefix.
    /// </summary>
    private static IBedrockModelAdapter CreateAdapter(string modelId)
    {
        // Detect model provider from model ID prefix
        if (modelId.StartsWith("anthropic.claude"))
        {
            return new ClaudeBedrockAdapter();
        }
        else if (modelId.StartsWith("amazon.titan"))
        {
            return new TitanBedrockAdapter();
        }
        else if (modelId.StartsWith("meta.llama"))
        {
            return new LlamaBedrockAdapter();
        }
        else
        {
            throw new NotSupportedException(
                $"Model {modelId} is not supported. Supported model prefixes: " +
                "anthropic.claude, amazon.titan, meta.llama");
        }
    }

    /// <summary>
    /// Disposes the Bedrock client.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _client?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
