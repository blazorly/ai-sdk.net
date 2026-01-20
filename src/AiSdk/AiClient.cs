using System.Runtime.CompilerServices;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Models;
using AiSdk.Utilities;

namespace AiSdk;

/// <summary>
/// Main entry point for AI SDK operations.
/// </summary>
public static class AiClient
{
    /// <summary>
    /// Generates text using a language model (non-streaming).
    /// </summary>
    /// <param name="model">The language model to use.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated text result.</returns>
    public static async Task<LanguageModelGenerateResult> GenerateTextAsync(
        ILanguageModel model,
        GenerateTextOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var messages = BuildMessages(options);

        var callOptions = new LanguageModelCallOptions
        {
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            StopSequences = options.StopSequences,
            Tools = options.Tools,
            ToolChoice = options.ToolChoice
        };

        return await model.GenerateAsync(callOptions, cancellationToken);
    }

    /// <summary>
    /// Generates text using a language model (streaming).
    /// </summary>
    /// <param name="model">The language model to use.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of text chunks.</returns>
    public static IAsyncEnumerable<LanguageModelStreamChunk> StreamTextAsync(
        ILanguageModel model,
        GenerateTextOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var messages = BuildMessages(options);

        var callOptions = new LanguageModelCallOptions
        {
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            StopSequences = options.StopSequences,
            Tools = options.Tools,
            ToolChoice = options.ToolChoice
        };

        return model.StreamAsync(callOptions, cancellationToken);
    }

    /// <summary>
    /// Generates a structured object from a language model using JSON schema.
    /// </summary>
    /// <typeparam name="T">The type of object to generate.</typeparam>
    /// <param name="model">The language model to use.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the generated object and metadata.</returns>
    public static async Task<GenerateObjectResult<T>> GenerateObjectAsync<T>(
        ILanguageModel model,
        GenerateObjectOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var messages = BuildObjectMessages(options);
        var schema = JsonSchemaGenerator.GenerateSchema<T>();

        var mode = options.Mode ?? "json";
        var toolName = options.Name ?? typeof(T).Name;
        var description = options.Description ?? $"Generated {typeof(T).Name} object";

        LanguageModelCallOptions callOptions;

        if (mode == "tool")
        {
            var tool = new ToolDefinition(
                Name: toolName,
                Description: description,
                Parameters: schema
            );

            callOptions = new LanguageModelCallOptions
            {
                Messages = messages,
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences,
                Tools = new[] { tool },
                ToolChoice = toolName
            };
        }
        else
        {
            var schemaJson = JsonSerializer.Serialize(schema);
            var systemMessage = options.System ?? "";
            systemMessage += $"\n\nYou must respond with valid JSON matching this schema:\n{schemaJson}";

            var messagesWithSchema = new List<Message> { new Message(MessageRole.System, systemMessage) };
            messagesWithSchema.AddRange(messages.Where(m => m.Role != MessageRole.System));

            callOptions = new LanguageModelCallOptions
            {
                Messages = messagesWithSchema,
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences
            };
        }

        var result = await model.GenerateAsync(callOptions, cancellationToken);

        T? parsedObject;
        string? rawText = null;

        try
        {
            if (mode == "tool" && result.ToolCalls?.Count > 0)
            {
                var toolCall = result.ToolCalls[0];
                parsedObject = JsonSerializer.Deserialize<T>(toolCall.Arguments);
                rawText = toolCall.Arguments.RootElement.GetRawText();
            }
            else
            {
                rawText = result.Text ?? string.Empty;
                var json = ExtractJsonFromMarkdown(rawText);
                parsedObject = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (parsedObject == null)
            {
                throw new JsonException($"Failed to deserialize response to type {typeof(T).Name}");
            }
        }
        catch (JsonException ex)
        {
            throw new JsonException(
                $"Failed to parse model output as {typeof(T).Name}. Raw output: {rawText}",
                ex);
        }

        return new GenerateObjectResult<T>
        {
            Object = parsedObject,
            RawText = rawText,
            FinishReason = result.FinishReason,
            Usage = result.Usage,
            Warnings = result.Warnings
        };
    }

    /// <summary>
    /// Streams a structured object from a language model, yielding partial objects as they're constructed.
    /// </summary>
    /// <typeparam name="T">The type of object to generate.</typeparam>
    /// <param name="model">The language model to use.</param>
    /// <param name="options">The streaming options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of object chunks.</returns>
    public static async IAsyncEnumerable<ObjectChunk<T>> StreamObjectAsync<T>(
        ILanguageModel model,
        StreamObjectOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var messages = BuildStreamMessages(options);
        var schema = JsonSchemaGenerator.GenerateSchema<T>();

        var mode = options.Mode ?? "json";
        var toolName = options.Name ?? typeof(T).Name;
        var description = options.Description ?? $"Generated {typeof(T).Name} object";

        LanguageModelCallOptions callOptions;

        if (mode == "tool")
        {
            var tool = new ToolDefinition(
                Name: toolName,
                Description: description,
                Parameters: schema
            );

            callOptions = new LanguageModelCallOptions
            {
                Messages = messages,
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences,
                Tools = new[] { tool },
                ToolChoice = toolName
            };
        }
        else
        {
            var schemaJson = JsonSerializer.Serialize(schema);
            var systemMessage = options.System ?? "";
            systemMessage += $"\n\nYou must respond with valid JSON matching this schema:\n{schemaJson}";

            var messagesWithSchema = new List<Message> { new Message(MessageRole.System, systemMessage) };
            messagesWithSchema.AddRange(messages.Where(m => m.Role != MessageRole.System));

            callOptions = new LanguageModelCallOptions
            {
                Messages = messagesWithSchema,
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences
            };
        }

        var accumulatedText = new System.Text.StringBuilder();
        LanguageModelStreamChunk? lastChunk = null;

        await foreach (var chunk in model.StreamAsync(callOptions, cancellationToken))
        {
            lastChunk = chunk;
            var delta = chunk.Delta ?? string.Empty;

            if (!string.IsNullOrEmpty(delta))
            {
                accumulatedText.Append(delta);
                options.OnChunk?.Invoke(delta);
            }

            var currentText = accumulatedText.ToString();
            T? partialObject = default;

            try
            {
                if (!string.IsNullOrWhiteSpace(currentText))
                {
                    var json = ExtractJsonFromMarkdown(currentText);
                    partialObject = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (JsonException)
            {
                // Partial JSON may not be valid yet
            }

            yield return new ObjectChunk<T>
            {
                Object = partialObject,
                Delta = delta,
                AccumulatedText = currentText,
                IsComplete = false
            };
        }

        var finalText = accumulatedText.ToString();
        T? finalObject = default;
        string? finalError = null;

        try
        {
            var json = ExtractJsonFromMarkdown(finalText);
            finalObject = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            finalError = ex.Message;
        }

        yield return new ObjectChunk<T>
        {
            Object = finalObject,
            Delta = null,
            AccumulatedText = finalText,
            IsComplete = true,
            Error = finalError,
            Usage = lastChunk?.Usage
        };
    }

    private static IReadOnlyList<Message> BuildMessages(GenerateTextOptions options)
    {
        var messages = new List<Message>();

        if (!string.IsNullOrEmpty(options.System))
        {
            messages.Add(new Message(MessageRole.System, options.System));
        }

        if (!string.IsNullOrEmpty(options.Prompt))
        {
            messages.Add(new Message(MessageRole.User, options.Prompt));
        }

        if (options.Messages is not null)
        {
            messages.AddRange(options.Messages);
        }

        if (messages.Count == 0)
        {
            throw new InvalidPromptError("Either Prompt or Messages must be provided.");
        }

        return messages;
    }

    private static IReadOnlyList<Message> BuildObjectMessages(GenerateObjectOptions options)
    {
        var messages = new List<Message>();

        if (!string.IsNullOrEmpty(options.System))
        {
            messages.Add(new Message(MessageRole.System, options.System));
        }

        if (!string.IsNullOrEmpty(options.Prompt))
        {
            messages.Add(new Message(MessageRole.User, options.Prompt));
        }

        if (options.Messages is not null)
        {
            messages.AddRange(options.Messages);
        }

        if (messages.Count == 0)
        {
            throw new InvalidPromptError("Either Prompt or Messages must be provided.");
        }

        return messages;
    }

    private static IReadOnlyList<Message> BuildStreamMessages(StreamObjectOptions options)
    {
        var messages = new List<Message>();

        if (!string.IsNullOrEmpty(options.System))
        {
            messages.Add(new Message(MessageRole.System, options.System));
        }

        if (!string.IsNullOrEmpty(options.Prompt))
        {
            messages.Add(new Message(MessageRole.User, options.Prompt));
        }

        if (options.Messages is not null)
        {
            messages.AddRange(options.Messages);
        }

        if (messages.Count == 0)
        {
            throw new InvalidPromptError("Either Prompt or Messages must be provided.");
        }

        return messages;
    }

    private static string ExtractJsonFromMarkdown(string text)
    {
        var trimmed = text.Trim();

        if (trimmed.StartsWith("```json"))
        {
            trimmed = trimmed.Substring(7);
        }
        else if (trimmed.StartsWith("```"))
        {
            trimmed = trimmed.Substring(3);
        }

        if (trimmed.EndsWith("```"))
        {
            trimmed = trimmed.Substring(0, trimmed.Length - 3);
        }

        return trimmed.Trim();
    }
}
