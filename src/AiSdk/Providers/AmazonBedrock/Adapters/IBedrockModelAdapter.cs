using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock.Models;

namespace AiSdk.Providers.AmazonBedrock.Adapters;

/// <summary>
/// Interface for adapting between AI SDK models and Bedrock model-specific formats.
/// </summary>
internal interface IBedrockModelAdapter
{
    /// <summary>
    /// Builds a request payload for the specific Bedrock model.
    /// </summary>
    /// <param name="options">The language model call options.</param>
    /// <returns>The model-specific request payload as JSON.</returns>
    string BuildRequest(LanguageModelCallOptions options);

    /// <summary>
    /// Parses a non-streaming response from the Bedrock model.
    /// </summary>
    /// <param name="responseJson">The JSON response from Bedrock.</param>
    /// <returns>The parsed language model result.</returns>
    LanguageModelGenerateResult ParseResponse(string responseJson);

    /// <summary>
    /// Parses a streaming response chunk from the Bedrock model.
    /// </summary>
    /// <param name="chunkJson">The JSON chunk from Bedrock streaming response.</param>
    /// <returns>The parsed stream chunk, or null if this chunk should be skipped.</returns>
    LanguageModelStreamChunk? ParseStreamChunk(string chunkJson);

    /// <summary>
    /// Gets whether this model supports streaming.
    /// </summary>
    bool SupportsStreaming { get; }
}
