using System.Diagnostics;

namespace AiSdk.Telemetry;

/// <summary>
/// Central ActivitySource for AI SDK telemetry.
/// Follows OpenTelemetry Semantic Conventions for GenAI.
/// </summary>
public static class AiSdkActivitySource
{
    /// <summary>
    /// The ActivitySource for all AI SDK operations.
    /// </summary>
    public static readonly ActivitySource Source = new("AiSdk", "1.2.1");

    /// <summary>
    /// Semantic convention attribute names for GenAI operations.
    /// Based on OpenTelemetry Semantic Conventions for GenAI.
    /// </summary>
    public static class Attributes
    {
        /// <summary>The GenAI system/provider (e.g., "openai", "anthropic").</summary>
        public const string GenAiSystem = "gen_ai.system";

        /// <summary>The model requested (e.g., "gpt-4o").</summary>
        public const string GenAiRequestModel = "gen_ai.request.model";

        /// <summary>The model used in the response (may differ from request).</summary>
        public const string GenAiResponseModel = "gen_ai.response.model";

        /// <summary>Max tokens requested.</summary>
        public const string GenAiRequestMaxTokens = "gen_ai.request.max_tokens";

        /// <summary>Temperature requested.</summary>
        public const string GenAiRequestTemperature = "gen_ai.request.temperature";

        /// <summary>Top-p requested.</summary>
        public const string GenAiRequestTopP = "gen_ai.request.top_p";

        /// <summary>Number of input tokens used.</summary>
        public const string GenAiUsageInputTokens = "gen_ai.usage.input_tokens";

        /// <summary>Number of output tokens used.</summary>
        public const string GenAiUsageOutputTokens = "gen_ai.usage.output_tokens";

        /// <summary>Finish reason(s) from the response.</summary>
        public const string GenAiResponseFinishReasons = "gen_ai.response.finish_reasons";

        /// <summary>The operation name (e.g., "chat", "embed").</summary>
        public const string GenAiOperationName = "gen_ai.operation.name";

        /// <summary>Whether streaming was used.</summary>
        public const string GenAiRequestStreaming = "gen_ai.request.streaming";

        /// <summary>Number of tool calls made.</summary>
        public const string GenAiToolCallCount = "gen_ai.tool_call.count";

        /// <summary>Number of steps in an agent loop.</summary>
        public const string GenAiAgentStepCount = "gen_ai.agent.step_count";
    }

    /// <summary>
    /// Starts an activity for a generate text operation.
    /// </summary>
    public static Activity? StartGenerateText(string provider, string modelId, int? maxTokens = null, double? temperature = null)
    {
        var activity = Source.StartActivity("gen_ai.generate_text", ActivityKind.Client);
        if (activity != null)
        {
            activity.SetTag(Attributes.GenAiSystem, provider);
            activity.SetTag(Attributes.GenAiRequestModel, modelId);
            activity.SetTag(Attributes.GenAiOperationName, "chat");
            activity.SetTag(Attributes.GenAiRequestStreaming, false);
            if (maxTokens.HasValue)
                activity.SetTag(Attributes.GenAiRequestMaxTokens, maxTokens.Value);
            if (temperature.HasValue)
                activity.SetTag(Attributes.GenAiRequestTemperature, temperature.Value);
        }
        return activity;
    }

    /// <summary>
    /// Starts an activity for a stream text operation.
    /// </summary>
    public static Activity? StartStreamText(string provider, string modelId, int? maxTokens = null, double? temperature = null)
    {
        var activity = Source.StartActivity("gen_ai.stream_text", ActivityKind.Client);
        if (activity != null)
        {
            activity.SetTag(Attributes.GenAiSystem, provider);
            activity.SetTag(Attributes.GenAiRequestModel, modelId);
            activity.SetTag(Attributes.GenAiOperationName, "chat");
            activity.SetTag(Attributes.GenAiRequestStreaming, true);
            if (maxTokens.HasValue)
                activity.SetTag(Attributes.GenAiRequestMaxTokens, maxTokens.Value);
            if (temperature.HasValue)
                activity.SetTag(Attributes.GenAiRequestTemperature, temperature.Value);
        }
        return activity;
    }

    /// <summary>
    /// Starts an activity for an embed operation.
    /// </summary>
    public static Activity? StartEmbed(string provider, string modelId)
    {
        var activity = Source.StartActivity("gen_ai.embed", ActivityKind.Client);
        if (activity != null)
        {
            activity.SetTag(Attributes.GenAiSystem, provider);
            activity.SetTag(Attributes.GenAiRequestModel, modelId);
            activity.SetTag(Attributes.GenAiOperationName, "embed");
        }
        return activity;
    }

    /// <summary>
    /// Records completion attributes on an activity.
    /// </summary>
    public static void RecordCompletion(Activity? activity, int? inputTokens, int? outputTokens, string? finishReason)
    {
        if (activity == null) return;

        if (inputTokens.HasValue)
            activity.SetTag(Attributes.GenAiUsageInputTokens, inputTokens.Value);
        if (outputTokens.HasValue)
            activity.SetTag(Attributes.GenAiUsageOutputTokens, outputTokens.Value);
        if (finishReason != null)
            activity.SetTag(Attributes.GenAiResponseFinishReasons, new[] { finishReason });
    }
}
