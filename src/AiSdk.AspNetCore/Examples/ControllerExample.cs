// This is a sample code file demonstrating controller-based usage of AiSdk.AspNetCore
// To use this, copy the relevant sections to your Controllers

#if EXAMPLES

using AiSdk;
using AiSdk.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AiSdk.AspNetCore.Examples;

/// <summary>
/// Example controller demonstrating various AI SDK usage patterns
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly ILanguageModel _model;
    private readonly ILogger<AiController> _logger;

    public AiController(ILanguageModel model, ILogger<AiController> logger)
    {
        _model = model;
        _logger = logger;
    }

    /// <summary>
    /// Generates a simple text response
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateText([FromBody] GenerateRequest request)
    {
        try
        {
            _logger.LogInformation("Generating text for prompt: {Prompt}", request.Prompt);

            var result = await AiClient.GenerateTextAsync(_model, new GenerateTextOptions
            {
                Prompt = request.Prompt,
                System = request.SystemMessage,
                MaxTokens = request.MaxTokens ?? 1000,
                Temperature = request.Temperature ?? 0.7,
                StopSequences = request.StopSequences
            });

            return Ok(new GenerateResponse
            {
                Text = result.Text,
                FinishReason = result.FinishReason,
                Usage = new UsageInfo
                {
                    PromptTokens = result.Usage?.PromptTokens ?? 0,
                    CompletionTokens = result.Usage?.CompletionTokens ?? 0,
                    TotalTokens = result.Usage?.TotalTokens ?? 0
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Streams a text response using Server-Sent Events
    /// </summary>
    [HttpPost("stream")]
    public async Task StreamText([FromBody] GenerateRequest request)
    {
        try
        {
            _logger.LogInformation("Streaming text for prompt: {Prompt}", request.Prompt);

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            await foreach (var chunk in AiClient.StreamTextAsync(_model, new GenerateTextOptions
            {
                Prompt = request.Prompt,
                System = request.SystemMessage,
                MaxTokens = request.MaxTokens ?? 1000,
                Temperature = request.Temperature ?? 0.7
            }))
            {
                var delta = chunk.Delta ?? string.Empty;
                if (!string.IsNullOrEmpty(delta))
                {
                    await Response.WriteAsync($"data: {delta}\n\n");
                    await Response.Body.FlushAsync();
                }
            }

            await Response.WriteAsync("data: [DONE]\n\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming text");
            await Response.WriteAsync($"event: error\ndata: {ex.Message}\n\n");
        }
    }

    /// <summary>
    /// Generates a structured object using JSON mode
    /// </summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(AnalysisResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> AnalyzeText([FromBody] AnalyzeTextRequest request)
    {
        try
        {
            _logger.LogInformation("Analyzing text: {Text}", request.Text);

            var result = await AiClient.GenerateObjectAsync<AnalysisResult>(_model, new GenerateObjectOptions
            {
                Prompt = $"Analyze the following text and provide a structured analysis:\n\n{request.Text}",
                Mode = "json",
                Temperature = 0.3
            });

            return Ok(result.Object);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing text");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Multi-turn conversation with message history
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        try
        {
            _logger.LogInformation("Processing chat with {MessageCount} messages", request.Messages.Count);

            var messages = request.Messages.Select(m => new Message(
                m.Role == "user" ? MessageRole.User : MessageRole.Assistant,
                m.Content
            )).ToList();

            var result = await AiClient.GenerateTextAsync(_model, new GenerateTextOptions
            {
                Messages = messages,
                System = request.SystemMessage,
                MaxTokens = request.MaxTokens ?? 1000,
                Temperature = request.Temperature ?? 0.7
            });

            return Ok(new ChatResponse
            {
                Message = result.Text,
                FinishReason = result.FinishReason,
                Usage = new UsageInfo
                {
                    PromptTokens = result.Usage?.PromptTokens ?? 0,
                    CompletionTokens = result.Usage?.CompletionTokens ?? 0,
                    TotalTokens = result.Usage?.TotalTokens ?? 0
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat");
            return BadRequest(new { error = ex.Message });
        }
    }
}

// Request/Response models

public record GenerateRequest
{
    public string Prompt { get; init; } = string.Empty;
    public string? SystemMessage { get; init; }
    public int? MaxTokens { get; init; }
    public double? Temperature { get; init; }
    public List<string>? StopSequences { get; init; }
}

public record GenerateResponse
{
    public string? Text { get; init; }
    public string? FinishReason { get; init; }
    public UsageInfo Usage { get; init; } = new();
}

public record AnalyzeTextRequest
{
    public string Text { get; init; } = string.Empty;
}

public record AnalysisResult
{
    public string Summary { get; init; } = string.Empty;
    public string Sentiment { get; init; } = string.Empty;
    public List<string> KeyTopics { get; init; } = new();
    public double Confidence { get; init; }
}

public record ChatRequest
{
    public List<ChatMessage> Messages { get; init; } = new();
    public string? SystemMessage { get; init; }
    public int? MaxTokens { get; init; }
    public double? Temperature { get; init; }
}

public record ChatMessage
{
    public string Role { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}

public record ChatResponse
{
    public string? Message { get; init; }
    public string? FinishReason { get; init; }
    public UsageInfo Usage { get; init; } = new();
}

public record UsageInfo
{
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
    public int TotalTokens { get; init; }
}

#endif
