using System.Diagnostics;
using System.Runtime.CompilerServices;
using AiSdk.Abstractions;
using AiSdk.Telemetry;
using Microsoft.Extensions.Logging;

namespace AiSdk.Middleware;

/// <summary>
/// Built-in middleware that adds OpenTelemetry spans and optional ILogger logging
/// to all language model calls.
/// </summary>
public class LoggingMiddleware : ILanguageModelMiddleware
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Creates a new LoggingMiddleware with optional ILogger.
    /// </summary>
    /// <param name="logger">Optional logger for structured logging.</param>
    public LoggingMiddleware(ILogger? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, Task<LanguageModelGenerateResult>> next,
        CancellationToken cancellationToken = default)
    {
        using var activity = AiSdkActivitySource.StartGenerateText(
            model.Provider, model.ModelId, options.MaxTokens, options.Temperature);

        var sw = Stopwatch.StartNew();
        _logger?.LogDebug("GenerateText starting: provider={Provider}, model={Model}", model.Provider, model.ModelId);

        try
        {
            var result = await next(options, cancellationToken);
            sw.Stop();

            AiSdkActivitySource.RecordCompletion(
                activity,
                result.Usage.InputTokens,
                result.Usage.OutputTokens,
                result.FinishReason.ToString());

            _logger?.LogInformation(
                "GenerateText completed in {Duration}ms: provider={Provider}, model={Model}, inputTokens={InputTokens}, outputTokens={OutputTokens}, finishReason={FinishReason}",
                sw.ElapsedMilliseconds, model.Provider, model.ModelId,
                result.Usage.InputTokens, result.Usage.OutputTokens, result.FinishReason);

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger?.LogError(ex, "GenerateText failed after {Duration}ms: provider={Provider}, model={Model}",
                sw.ElapsedMilliseconds, model.Provider, model.ModelId);
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, IAsyncEnumerable<LanguageModelStreamChunk>> next,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = AiSdkActivitySource.StartStreamText(
            model.Provider, model.ModelId, options.MaxTokens, options.Temperature);

        var sw = Stopwatch.StartNew();
        _logger?.LogDebug("StreamText starting: provider={Provider}, model={Model}", model.Provider, model.ModelId);

        LanguageModelStreamChunk? lastChunk = null;

        await foreach (var chunk in next(options, cancellationToken))
        {
            lastChunk = chunk;
            yield return chunk;

            if (chunk.Type == ChunkType.Finish)
            {
                sw.Stop();
                AiSdkActivitySource.RecordCompletion(
                    activity,
                    chunk.Usage?.InputTokens,
                    chunk.Usage?.OutputTokens,
                    chunk.FinishReason?.ToString());

                _logger?.LogInformation(
                    "StreamText completed in {Duration}ms: provider={Provider}, model={Model}, finishReason={FinishReason}",
                    sw.ElapsedMilliseconds, model.Provider, model.ModelId, chunk.FinishReason);
            }
        }
    }
}
