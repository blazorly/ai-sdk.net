using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace AiSdk.Middleware;

/// <summary>
/// Middleware that caches language model responses using IDistributedCache.
/// Caches non-streaming responses based on a hash of the input messages and options.
/// Streaming responses pass through without caching.
/// </summary>
public class CachingMiddleware : ILanguageModelMiddleware
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMiddleware"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache to use.</param>
    /// <param name="cacheOptions">Optional cache entry options (expiration, etc.).</param>
    public CachingMiddleware(
        IDistributedCache cache,
        DistributedCacheEntryOptions? cacheOptions = null)
    {
        ArgumentNullException.ThrowIfNull(cache);
        _cache = cache;
        _cacheOptions = cacheOptions ?? new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };
    }

    /// <inheritdoc/>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, Task<LanguageModelGenerateResult>> next,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(model, options);

        // Try to get from cache
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<CachedGenerateResult>(cached);
            if (cachedResult != null)
            {
                return new LanguageModelGenerateResult
                {
                    Text = cachedResult.Text,
                    FinishReason = cachedResult.FinishReason,
                    Usage = new Usage(
                        InputTokens: cachedResult.InputTokens,
                        OutputTokens: cachedResult.OutputTokens,
                        TotalTokens: cachedResult.TotalTokens),
                    ToolCalls = cachedResult.ToolCalls
                };
            }
        }

        // Cache miss - call the model
        var result = await next(options, cancellationToken);

        // Cache the result
        var toCache = new CachedGenerateResult
        {
            Text = result.Text,
            FinishReason = result.FinishReason,
            InputTokens = result.Usage.InputTokens,
            OutputTokens = result.Usage.OutputTokens,
            TotalTokens = result.Usage.TotalTokens,
            ToolCalls = result.ToolCalls
        };

        var json = JsonSerializer.Serialize(toCache);
        await _cache.SetStringAsync(cacheKey, json, _cacheOptions, cancellationToken);

        return result;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, IAsyncEnumerable<LanguageModelStreamChunk>> next,
        CancellationToken cancellationToken = default)
    {
        // Streaming responses are not cached - pass through
        return next(options, cancellationToken);
    }

    private static string BuildCacheKey(ILanguageModel model, LanguageModelCallOptions options)
    {
        var sb = new StringBuilder();
        sb.Append(model.Provider);
        sb.Append(':');
        sb.Append(model.ModelId);
        sb.Append(':');

        // Include relevant options in the cache key
        foreach (var message in options.Messages)
        {
            sb.Append(message.Role.ToString());
            sb.Append('|');
            sb.Append(message.Content);
            sb.Append('|');
        }

        if (options.MaxTokens.HasValue) sb.Append($"mt:{options.MaxTokens}|");
        if (options.Temperature.HasValue) sb.Append($"t:{options.Temperature}|");
        if (options.TopP.HasValue) sb.Append($"tp:{options.TopP}|");

        if (options.Tools != null)
        {
            foreach (var tool in options.Tools)
            {
                sb.Append($"tool:{tool.Name}|");
            }
        }

        // Hash the key for consistent size
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return $"aisdk:cache:{Convert.ToHexString(hash)}";
    }

    private record CachedGenerateResult
    {
        public string? Text { get; init; }
        public FinishReason FinishReason { get; init; }
        public int? InputTokens { get; init; }
        public int? OutputTokens { get; init; }
        public int? TotalTokens { get; init; }
        public IReadOnlyList<ToolCall>? ToolCalls { get; init; }
    }
}
