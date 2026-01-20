using System.Text;
using System.Text.Json;
using AiSdk;
using AiSdk.Abstractions;
using Microsoft.AspNetCore.Mvc;
using MinimalApiExample;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AI SDK Minimal API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register AI SDK services
// For production, configure a real provider:
// builder.Services.AddSingleton<ILanguageModel>(sp =>
// {
//     var apiKey = builder.Configuration["OpenAI:ApiKey"]
//         ?? throw new InvalidOperationException("OpenAI API key not configured");
//     return new OpenAIProvider(apiKey: apiKey).ChatModel("gpt-4");
// });

// For demo/testing, use the mock model (no API key required)
builder.Services.AddSingleton<ILanguageModel, MockLanguageModel>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// ============================================================================
// Health Check Endpoint
// ============================================================================

app.MapGet("/health", () =>
{
    logger.LogInformation("Health check requested");
    return Results.Ok(new
    {
        Status = "Healthy",
        Timestamp = DateTime.UtcNow,
        Service = "AI SDK Minimal API",
        Version = "1.0.0"
    });
})
.WithName("HealthCheck")
.WithTags("Health")
;

// ============================================================================
// Chat Completion Endpoint (Non-Streaming)
// ============================================================================

app.MapPost("/api/chat", async (
    [FromBody] ChatRequest request,
    [FromServices] ILanguageModel model,
    [FromServices] ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    try
    {
        logger.LogInformation("Chat completion requested: {Message}", request.Message);

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return Results.BadRequest(new { Error = "Message is required" });
        }

        var options = new GenerateTextOptions
        {
            System = request.SystemMessage ?? "You are a helpful AI assistant.",
            Prompt = request.Message,
            MaxTokens = request.MaxTokens ?? 1000,
            Temperature = request.Temperature ?? 0.7
        };

        var result = await AiClient.GenerateTextAsync(model, options, cancellationToken);

        var response = new ChatResponse
        {
            Message = result.Text ?? string.Empty,
            FinishReason = result.FinishReason.ToString(),
            Usage = result.Usage != null ? new UsageInfo
            {
                InputTokens = result.Usage.InputTokens ?? 0,
                OutputTokens = result.Usage.OutputTokens ?? 0,
                TotalTokens = result.Usage.TotalTokens ?? 0
            } : null,
            Model = model.ModelId,
            Provider = model.Provider
        };

        logger.LogInformation("Chat completion successful. Tokens used: {Tokens}", result.Usage?.TotalTokens);

        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing chat request");
        return Results.Problem(
            title: "Chat Error",
            detail: ex.Message,
            statusCode: 500
        );
    }
})
.WithName("ChatCompletion")
.WithTags("Chat")
;

// ============================================================================
// Streaming Chat Endpoint (Server-Sent Events)
// ============================================================================

app.MapPost("/api/chat/stream", async (
    [FromBody] ChatRequest request,
    [FromServices] ILanguageModel model,
    [FromServices] ILogger<Program> logger,
    HttpContext context,
    CancellationToken cancellationToken) =>
{
    try
    {
        logger.LogInformation("Streaming chat requested: {Message}", request.Message);

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { Error = "Message is required" });
            return;
        }

        // Set up Server-Sent Events
        context.Response.Headers.Append("Content-Type", "text/event-stream");
        context.Response.Headers.Append("Cache-Control", "no-cache");
        context.Response.Headers.Append("Connection", "keep-alive");

        var options = new GenerateTextOptions
        {
            System = request.SystemMessage ?? "You are a helpful AI assistant.",
            Prompt = request.Message,
            MaxTokens = request.MaxTokens ?? 1000,
            Temperature = request.Temperature ?? 0.7
        };

        var tokenCount = 0;

        await foreach (var chunk in AiClient.StreamTextAsync(model, options, cancellationToken))
        {
            if (chunk.Delta != null)
            {
                tokenCount++;

                // Send text delta
                var deltaEvent = new StreamEvent
                {
                    Type = "delta",
                    Data = new { Text = chunk.Delta }
                };

                await WriteServerSentEvent(context.Response, deltaEvent);
                await context.Response.Body.FlushAsync(cancellationToken);
            }

            if (chunk.Type == ChunkType.Finish)
            {
                // Send completion event with metadata
                var completeEvent = new StreamEvent
                {
                    Type = "complete",
                    Data = new
                    {
                        FinishReason = chunk.FinishReason?.ToString(),
                        Usage = chunk.Usage != null ? new UsageInfo
                        {
                            InputTokens = chunk.Usage.InputTokens ?? 0,
                            OutputTokens = chunk.Usage.OutputTokens ?? 0,
                            TotalTokens = chunk.Usage.TotalTokens ?? 0
                        } : null,
                        Model = model.ModelId,
                        Provider = model.Provider
                    }
                };

                await WriteServerSentEvent(context.Response, completeEvent);
                await context.Response.Body.FlushAsync(cancellationToken);

                logger.LogInformation("Streaming completed. Total chunks: {Count}", tokenCount);
            }
        }
    }
    catch (OperationCanceledException)
    {
        logger.LogInformation("Streaming cancelled by client");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during streaming");

        var errorEvent = new StreamEvent
        {
            Type = "error",
            Data = new { Message = ex.Message }
        };

        await WriteServerSentEvent(context.Response, errorEvent);
    }
})
.WithName("StreamingChat")
.WithTags("Chat")
;

// ============================================================================
// Information Endpoint
// ============================================================================

app.MapGet("/api/info", ([FromServices] ILanguageModel model) =>
{
    return Results.Ok(new
    {
        Service = "AI SDK Minimal API Example",
        Description = "Demonstrates AI SDK integration with ASP.NET Core Minimal APIs",
        Model = new
        {
            Provider = model.Provider,
            ModelId = model.ModelId,
            SpecVersion = model.SpecificationVersion
        },
        Endpoints = new[]
        {
            new { Method = "GET", Path = "/health", Description = "Health check endpoint" },
            new { Method = "GET", Path = "/api/info", Description = "API information" },
            new { Method = "POST", Path = "/api/chat", Description = "Non-streaming chat completion" },
            new { Method = "POST", Path = "/api/chat/stream", Description = "Streaming chat with SSE" }
        },
        Documentation = "/swagger"
    });
})
.WithName("ApiInfo")
.WithTags("Info")
;

// ============================================================================
// Helper Methods
// ============================================================================

static async Task WriteServerSentEvent(HttpResponse response, StreamEvent evt)
{
    var json = JsonSerializer.Serialize(evt.Data);
    var eventData = $"event: {evt.Type}\ndata: {json}\n\n";
    var bytes = Encoding.UTF8.GetBytes(eventData);
    await response.Body.WriteAsync(bytes);
}

// Start the application
logger.LogInformation("Starting AI SDK Minimal API on {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Swagger UI available at: /swagger");

app.Run();

// ============================================================================
// Request/Response Models
// ============================================================================

/// <summary>
/// Request model for chat endpoints.
/// </summary>
public record ChatRequest
{
    /// <summary>
    /// The user's message/question.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Optional system message to set behavior.
    /// </summary>
    public string? SystemMessage { get; init; }

    /// <summary>
    /// Maximum tokens to generate (default: 1000).
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Temperature for randomness (0.0 to 2.0, default: 0.7).
    /// </summary>
    public double? Temperature { get; init; }
}

/// <summary>
/// Response model for non-streaming chat.
/// </summary>
public record ChatResponse
{
    public required string Message { get; init; }
    public required string FinishReason { get; init; }
    public UsageInfo? Usage { get; init; }
    public required string Model { get; init; }
    public required string Provider { get; init; }
}

/// <summary>
/// Token usage information.
/// </summary>
public record UsageInfo
{
    public int InputTokens { get; init; }
    public int OutputTokens { get; init; }
    public int TotalTokens { get; init; }
}

/// <summary>
/// Server-sent event structure.
/// </summary>
public record StreamEvent
{
    public required string Type { get; init; }
    public required object Data { get; init; }
}
