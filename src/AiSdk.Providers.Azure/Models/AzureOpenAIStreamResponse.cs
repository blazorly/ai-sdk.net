using System.Text.Json.Serialization;

namespace AiSdk.Providers.Azure.Models;

/// <summary>
/// Azure OpenAI streaming chat completion response chunk.
/// </summary>
internal record AzureOpenAIStreamResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("created")]
    public required long Created { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("choices")]
    public required List<AzureOpenAIStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public AzureOpenAIUsage? Usage { get; init; }
}

/// <summary>
/// Azure OpenAI streaming choice.
/// </summary>
internal record AzureOpenAIStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required AzureOpenAIDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Azure OpenAI delta (partial content in streaming).
/// </summary>
internal record AzureOpenAIDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<AzureOpenAIToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Azure OpenAI tool call delta.
/// </summary>
internal record AzureOpenAIToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public AzureOpenAIFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Azure OpenAI function call delta.
/// </summary>
internal record AzureOpenAIFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
