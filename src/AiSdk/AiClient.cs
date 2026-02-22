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
    /// When MaxSteps is not set (default), performs a single generation call.
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

        // If MaxSteps is set, delegate to the multi-step version
        if (options.MaxSteps.HasValue && options.MaxSteps.Value > 1)
        {
            var multiStepResult = await GenerateTextWithStepsAsync(model, options, cancellationToken);
            // Return the last step's result for backward compatibility
            return new LanguageModelGenerateResult
            {
                Text = multiStepResult.Text,
                FinishReason = multiStepResult.FinishReason,
                Usage = multiStepResult.Usage,
                ToolCalls = multiStepResult.ToolCalls,
                ProviderMetadata = multiStepResult.ProviderMetadata,
                Warnings = multiStepResult.Warnings
            };
        }

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
    /// Generates text with multi-step agent capabilities.
    /// Automatically executes tool calls and feeds results back to the model
    /// until the model stops calling tools or MaxSteps is reached.
    /// </summary>
    /// <param name="model">The language model to use.</param>
    /// <param name="options">The generation options (MaxSteps and ToolExecutors should be set).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing all steps and the final output.</returns>
    public static async Task<GenerateTextResult> GenerateTextWithStepsAsync(
        ILanguageModel model,
        GenerateTextOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(options);

        var maxSteps = options.MaxSteps ?? 1;
        var messages = new List<Message>(BuildMessages(options));
        var steps = new List<StepResult>();
        int? totalInputTokens = 0;
        int? totalOutputTokens = 0;
        int? totalTokens = 0;

        for (var step = 0; step < maxSteps; step++)
        {
            var callOptions = new LanguageModelCallOptions
            {
                Messages = messages.AsReadOnly(),
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences,
                Tools = options.Tools,
                ToolChoice = options.ToolChoice
            };

            var result = await model.GenerateAsync(callOptions, cancellationToken);

            totalInputTokens = (totalInputTokens ?? 0) + (result.Usage.InputTokens ?? 0);
            totalOutputTokens = (totalOutputTokens ?? 0) + (result.Usage.OutputTokens ?? 0);
            totalTokens = (totalTokens ?? 0) + (result.Usage.TotalTokens ?? 0);

            var stepResult = new StepResult
            {
                StepNumber = step,
                Text = result.Text,
                FinishReason = result.FinishReason,
                Usage = result.Usage,
                ToolCalls = result.ToolCalls
            };

            // If the model didn't call any tools, we're done
            if (result.FinishReason != FinishReason.ToolCalls ||
                result.ToolCalls == null ||
                result.ToolCalls.Count == 0 ||
                options.ToolExecutors == null)
            {
                steps.Add(stepResult);
                options.OnStepFinish?.Invoke(stepResult);
                break;
            }

            // Execute each tool call
            var toolResults = new List<ToolResult>();

            // Add assistant message with tool calls
            messages.Add(new Message(MessageRole.Assistant, result.Text ?? string.Empty)
            {
                Metadata = new Dictionary<string, object>
                {
                    ["toolCalls"] = result.ToolCalls
                }
            });

            foreach (var toolCall in result.ToolCalls)
            {
                if (options.ToolExecutors.TryGetValue(toolCall.ToolName, out var executor))
                {
                    var toolResultString = await executor.ExecuteAsync(toolCall.Arguments, cancellationToken);

                    toolResults.Add(new ToolResult
                    {
                        ToolCallId = toolCall.ToolCallId,
                        ToolName = toolCall.ToolName,
                        Result = toolResultString
                    });

                    // Add tool result message
                    messages.Add(new Message(MessageRole.Tool, toolResultString, toolCall.ToolCallId));
                }
                else
                {
                    throw new NoSuchToolError($"Tool '{toolCall.ToolName}' not found in ToolExecutors")
                    {
                        ToolName = toolCall.ToolName
                    };
                }
            }

            stepResult = stepResult with { ToolResults = toolResults };
            steps.Add(stepResult);
            options.OnStepFinish?.Invoke(stepResult);
        }

        var lastStep = steps[^1];

        return new GenerateTextResult
        {
            Text = lastStep.Text,
            FinishReason = lastStep.FinishReason,
            Usage = new Usage(totalInputTokens, totalOutputTokens, totalTokens),
            ToolCalls = lastStep.ToolCalls,
            Steps = steps.AsReadOnly(),
            Messages = messages.AsReadOnly(),
            Warnings = null
        };
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

    /// <summary>
    /// Generates an embedding for a single text input.
    /// </summary>
    /// <param name="model">The embedding model to use.</param>
    /// <param name="input">The text to embed.</param>
    /// <param name="options">Optional embedding options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embedding result containing the vector and usage statistics.</returns>
    public static Task<EmbeddingResult> EmbedAsync(
        IEmbeddingModel model,
        string input,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(input);

        return model.EmbedAsync(input, options, cancellationToken);
    }

    /// <summary>
    /// Generates embeddings for multiple text inputs in a batch.
    /// </summary>
    /// <param name="model">The embedding model to use.</param>
    /// <param name="inputs">The texts to embed.</param>
    /// <param name="options">Optional embedding options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The batch embedding result containing all vectors and usage statistics.</returns>
    public static Task<BatchEmbeddingResult> EmbedManyAsync(
        IEmbeddingModel model,
        IEnumerable<string> inputs,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(inputs);

        return model.EmbedManyAsync(inputs, options, cancellationToken);
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
