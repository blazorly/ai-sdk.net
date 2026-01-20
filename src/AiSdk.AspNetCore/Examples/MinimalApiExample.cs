// This is a sample code file demonstrating usage of AiSdk.AspNetCore
// To use this, copy the relevant sections to your Program.cs

#if EXAMPLES

using AiSdk;
using AiSdk.Abstractions;
using AiSdk.AspNetCore.Configuration;
using AiSdk.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AiSdk.AspNetCore.Examples;

/// <summary>
/// Example demonstrating minimal API setup with AiSdk.AspNetCore
/// </summary>
public class MinimalApiExample
{
    public static void ConfigureServices()
    {
        var builder = WebApplication.CreateBuilder();

        // Example 1: Basic configuration with inline options
        builder.Services.AddAiSdk(options =>
        {
            options.DefaultProvider = "openai";
            options.EnableHealthChecks = true;
            options.EnableTelemetry = true;

            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "your-openai-api-key",
                DefaultModel = "gpt-4",
                TimeoutSeconds = 30,
                Enabled = true
            };

            options.Providers["anthropic"] = new ProviderConfiguration
            {
                ApiKey = "your-anthropic-api-key",
                DefaultModel = "claude-3-opus-20240229",
                Enabled = true
            };
        });

        // Example 2: Configuration from appsettings.json
        // builder.Services.AddAiSdk(builder.Configuration.GetSection("AiSdk"));

        // Add health checks
        builder.Services.AddHealthChecks()
            .AddAiSdkHealthCheck(
                name: "ai-sdk",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "ai", "ready" });

        var app = builder.Build();

        // Add streaming middleware
        app.UseAiSdkStreaming();

        // Health check endpoint
        app.MapHealthChecks("/health");

        // Example endpoint: Simple text generation
        app.MapPost("/api/chat", async (
            [FromBody] ChatRequest request,
            [FromServices] ILanguageModel model) =>
        {
            var result = await AiClient.GenerateTextAsync(model, new GenerateTextOptions
            {
                Prompt = request.Message,
                MaxTokens = 1000,
                Temperature = 0.7
            });

            return Results.Ok(new { response = result.Text, usage = result.Usage });
        });

        // Example endpoint: Streaming response
        app.MapPost("/api/chat/stream", async (
            [FromBody] ChatRequest request,
            [FromServices] ILanguageModel model,
            HttpContext context) =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers["Cache-Control"] = "no-cache";
            context.Response.Headers["Connection"] = "keep-alive";

            await foreach (var chunk in AiClient.StreamTextAsync(model, new GenerateTextOptions
            {
                Prompt = request.Message,
                MaxTokens = 1000,
                Temperature = 0.7
            }))
            {
                var delta = chunk.Delta ?? string.Empty;
                await context.Response.WriteAsync($"data: {delta}\n\n");
                await context.Response.Body.FlushAsync();
            }

            await context.Response.WriteAsync("data: [DONE]\n\n");
        });

        // Example endpoint: Structured object generation
        app.MapPost("/api/analyze", async (
            [FromBody] AnalyzeRequest request,
            [FromServices] ILanguageModel model) =>
        {
            var result = await AiClient.GenerateObjectAsync<SentimentAnalysis>(model, new GenerateObjectOptions
            {
                Prompt = $"Analyze the sentiment of this text: {request.Text}",
                Mode = "json",
                Temperature = 0.3
            });

            return Results.Ok(result.Object);
        });

        app.Run();
    }
}

/// <summary>
/// Sample request model for chat endpoints
/// </summary>
public record ChatRequest
{
    public string Message { get; init; } = string.Empty;
    public int? MaxTokens { get; init; }
    public double? Temperature { get; init; }
}

/// <summary>
/// Sample request model for analysis endpoint
/// </summary>
public record AnalyzeRequest
{
    public string Text { get; init; } = string.Empty;
}

/// <summary>
/// Sample response model for sentiment analysis
/// </summary>
public record SentimentAnalysis
{
    public string Sentiment { get; init; } = string.Empty;
    public double Confidence { get; init; }
    public string Reasoning { get; init; } = string.Empty;
}

#endif
