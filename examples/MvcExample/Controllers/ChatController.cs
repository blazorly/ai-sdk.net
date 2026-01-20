using Microsoft.AspNetCore.Mvc;
using AiSdk;
using AiSdk.Abstractions;
using MvcExample.Models;

namespace MvcExample.Controllers;

/// <summary>
/// Controller for handling chat interactions with the AI model.
/// </summary>
public class ChatController : Controller
{
    private readonly ILanguageModel _model;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILanguageModel model, ILogger<ChatController> logger)
    {
        _model = model;
        _logger = logger;
    }

    /// <summary>
    /// Displays the chat interface.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles non-streaming chat messages (returns JSON response).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new ChatResponse
                {
                    IsError = true,
                    ErrorMessage = "Message cannot be empty"
                });
            }

            _logger.LogInformation("Generating response for message: {Message}", request.Message);

            // Build the options for the AI call
            var options = new GenerateTextOptions
            {
                Prompt = request.Message,
                System = request.SystemPrompt,
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature
            };

            // Generate the response
            var result = await AiClient.GenerateTextAsync(_model, options, cancellationToken);

            // Map to response model
            var response = new ChatResponse
            {
                Text = result.Text ?? string.Empty,
                FinishReason = result.FinishReason.ToString(),
                Usage = result.Usage != null ? new UsageInfo
                {
                    InputTokens = result.Usage.InputTokens ?? 0,
                    OutputTokens = result.Usage.OutputTokens ?? 0,
                    TotalTokens = result.Usage.TotalTokens ?? 0
                } : null,
                IsError = false
            };

            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response");
            return StatusCode(500, new ChatResponse
            {
                IsError = true,
                ErrorMessage = "An error occurred while generating the response. Please try again."
            });
        }
    }

    /// <summary>
    /// Handles streaming chat messages (Server-Sent Events).
    /// </summary>
    [HttpPost]
    public async Task StreamMessage([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Message cannot be empty", cancellationToken);
                return;
            }

            _logger.LogInformation("Streaming response for message: {Message}", request.Message);

            // Set up Server-Sent Events headers
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            // Build the options for the AI call
            var options = new GenerateTextOptions
            {
                Prompt = request.Message,
                System = request.SystemPrompt,
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature
            };

            // Stream the response
            await foreach (var chunk in AiClient.StreamTextAsync(_model, options, cancellationToken))
            {
                if (!string.IsNullOrEmpty(chunk.Delta))
                {
                    // Send text delta as SSE
                    await Response.WriteAsync($"data: {{\"type\":\"delta\",\"text\":\"{EscapeJson(chunk.Delta)}\"}}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
                else if (chunk.Usage != null)
                {
                    // Send usage information
                    var usageJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        type = "usage",
                        usage = new
                        {
                            inputTokens = chunk.Usage.InputTokens,
                            outputTokens = chunk.Usage.OutputTokens,
                            totalTokens = chunk.Usage.TotalTokens
                        }
                    });
                    await Response.WriteAsync($"data: {usageJson}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }

            // Send completion event
            await Response.WriteAsync("data: {\"type\":\"done\"}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming response");

            // Send error event
            var errorJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "error",
                message = "An error occurred while generating the response"
            });
            await Response.WriteAsync($"data: {errorJson}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Escapes special characters for JSON string values.
    /// </summary>
    private static string EscapeJson(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
