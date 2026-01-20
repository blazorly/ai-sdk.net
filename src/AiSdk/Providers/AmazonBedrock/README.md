# AiSdk.Providers.AmazonBedrock

Amazon Bedrock provider for AI SDK .NET - a multi-provider aggregator that provides unified access to 7+ AI model providers through AWS, including Anthropic Claude, Amazon Titan, Meta Llama, Cohere, Mistral, and more.

## Features

- **Multi-Provider Access** - Single API for 7+ model providers through AWS Bedrock
- **Anthropic Claude** - Claude 3.5 Sonnet, Claude 3 Opus/Sonnet/Haiku with full tool use support
- **Amazon Titan** - Titan Text Express and Lite models
- **Meta Llama** - Llama 3.1 70B and 8B Instruct models
- **Streaming Support** - Real-time streaming for supported models (Claude)
- **Tool Use** - Function calling support for Claude models
- **AWS Authentication** - Supports AWS credentials, IAM roles, and temporary credentials
- **Multi-Region** - Deploy across all AWS regions where Bedrock is available
- **Flexible Configuration** - Access keys, IAM roles, session tokens, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.AmazonBedrock
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock;

// Create a Claude 3.5 Sonnet model on Bedrock
var model = AmazonBedrockProvider.Claude35Sonnet(
    region: "us-east-1",
    accessKeyId: "YOUR_AWS_ACCESS_KEY",
    secretAccessKey: "YOUR_AWS_SECRET_KEY"
);

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain quantum computing in simple terms")
    },
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration with Credentials

```csharp
var config = new AmazonBedrockConfiguration
{
    Region = "us-east-1",                    // Required: AWS region
    AccessKeyId = "AKIA...",                 // Optional: AWS access key
    SecretAccessKey = "...",                 // Optional: AWS secret key
    SessionToken = "...",                    // Optional: For temporary credentials
    TimeoutSeconds = 120                     // Optional: Request timeout (default: AWS SDK default)
};

var model = AmazonBedrockProvider.CreateModel(
    "anthropic.claude-3-5-sonnet-20241022-v2:0",
    config
);
```

### Using IAM Roles (Recommended for EC2/ECS/Lambda)

```csharp
// When running on AWS infrastructure (EC2, ECS, Lambda), you can omit credentials
// The SDK will automatically use the IAM role attached to the instance/container
var config = new AmazonBedrockConfiguration
{
    Region = "us-east-1"
    // No AccessKeyId or SecretAccessKey needed - uses IAM role
};

var model = AmazonBedrockProvider.CreateModel(
    "anthropic.claude-3-5-sonnet-20241022-v2:0",
    config
);
```

### Environment Variables

```csharp
var config = new AmazonBedrockConfiguration
{
    Region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1",
    AccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
    SecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
};
```

## Supported Models

### Anthropic Claude Models (Full Support)

```csharp
// Claude 3.5 Sonnet v2 - Latest and most capable
var claude35 = AmazonBedrockProvider.Claude35Sonnet("us-east-1");
// Model ID: anthropic.claude-3-5-sonnet-20241022-v2:0

// Claude 3 Opus - Most capable Claude 3 model
var claude3Opus = AmazonBedrockProvider.Claude3Opus("us-east-1");
// Model ID: anthropic.claude-3-opus-20240229-v1:0

// Claude 3 Sonnet - Balanced performance
var claude3Sonnet = AmazonBedrockProvider.Claude3Sonnet("us-east-1");
// Model ID: anthropic.claude-3-sonnet-20240229-v1:0

// Claude 3 Haiku - Fast and compact
var claude3Haiku = AmazonBedrockProvider.Claude3Haiku("us-east-1");
// Model ID: anthropic.claude-3-haiku-20240307-v1:0

// Or use custom model ID
var model = AmazonBedrockProvider.CreateModel(
    "anthropic.claude-3-5-sonnet-20241022-v2:0",
    "us-west-2"
);
```

**Claude Features:**
- Streaming: ✅ Full support
- Tool Use: ✅ Full support
- Vision: ✅ Supported (image URLs)
- System Messages: ✅ Native support
- Max Output Tokens: 4096 (configurable)

### Amazon Titan Models

```csharp
// Titan Text Express - Fast text generation
var titanExpress = AmazonBedrockProvider.TitanTextExpress("us-east-1");
// Model ID: amazon.titan-text-express-v1

// Titan Text Lite - Lightweight model
var titanLite = AmazonBedrockProvider.TitanTextLite("us-east-1");
// Model ID: amazon.titan-text-lite-v1
```

**Titan Features:**
- Streaming: ❌ Not supported
- Tool Use: ❌ Not supported
- Vision: ❌ Not supported
- System Messages: ✅ Supported (converted to prompt)
- Max Output Tokens: 8192

### Meta Llama Models

```csharp
// Llama 3.1 70B Instruct - High performance
var llama70b = AmazonBedrockProvider.Llama31_70B("us-east-1");
// Model ID: meta.llama3-1-70b-instruct-v1:0

// Llama 3.1 8B Instruct - Compact and efficient
var llama8b = AmazonBedrockProvider.Llama31_8B("us-east-1");
// Model ID: meta.llama3-1-8b-instruct-v1:0
```

**Llama Features:**
- Streaming: ❌ Not supported on Bedrock
- Tool Use: ❌ Not supported
- Vision: ❌ Not supported
- System Messages: ✅ Supported
- Max Output Tokens: 2048

## Usage Examples

### Basic Text Generation

```csharp
var model = AmazonBedrockProvider.Claude35Sonnet("us-east-1");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a haiku about programming")
    },
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
Console.WriteLine($"Tokens used: {result.Usage?.TotalTokens}");
```

### Streaming (Claude models only)

```csharp
var model = AmazonBedrockProvider.Claude35Sonnet("us-east-1");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a short story about AI")
    },
    MaxTokens = 2048
};

await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
    else if (chunk.Type == ChunkType.Finish)
    {
        Console.WriteLine($"\n\nFinish reason: {chunk.FinishReason}");
        Console.WriteLine($"Tokens used: {chunk.Usage?.TotalTokens}");
    }
}
```

### Tool Use (Claude models only)

```csharp
var model = AmazonBedrockProvider.Claude35Sonnet("us-east-1");

var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonSchema.FromType<WeatherRequest>()
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What's the weather in San Francisco?")
    },
    Tools = tools,
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        var args = toolCall.Arguments.RootElement;

        // Execute tool and return result
        var toolResult = ExecuteWeatherTool(args);

        // Continue conversation with tool result
        var followUpOptions = new LanguageModelCallOptions
        {
            Messages = new List<Message>
            {
                new Message(MessageRole.User, "What's the weather in San Francisco?"),
                new Message(MessageRole.Assistant, result.Text, ToolCalls: result.ToolCalls),
                new Message(
                    Role: MessageRole.Tool,
                    Content: toolResult,
                    Name: toolCall.ToolCallId
                )
            },
            Tools = tools,
            MaxTokens = 1024
        };

        var finalResult = await model.GenerateAsync(followUpOptions);
        Console.WriteLine(finalResult.Text);
    }
}
```

### Multi-turn Conversation

```csharp
var model = AmazonBedrockProvider.Claude35Sonnet("us-east-1");

var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful AI assistant."),
    new Message(MessageRole.User, "What is the capital of France?"),
    new Message(MessageRole.Assistant, "The capital of France is Paris."),
    new Message(MessageRole.User, "What's its population?")
};

var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 4096,                          // Maximum tokens to generate
    Temperature = 0.7,                         // Randomness (0.0 - 1.0)
    TopP = 0.9,                                // Nucleus sampling
    StopSequences = new List<string> { "\n\n" } // Stop generation at these sequences
};

var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.AmazonBedrock.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (AmazonBedrockException ex) when (ex.StatusCode == 403)
{
    Console.WriteLine("Access denied. Check IAM permissions for Bedrock.");
}
catch (AmazonBedrockException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine($"Throttled. Error code: {ex.ErrorCode}");
}
catch (AmazonBedrockException ex)
{
    Console.WriteLine($"Bedrock error: {ex.Message}");
    Console.WriteLine($"Model: {ex.ModelId}");
    Console.WriteLine($"Status: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"Feature not supported: {ex.Message}");
}
```

### Cancellation Support

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

try
{
    var result = await model.GenerateAsync(options, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request was cancelled");
}
```

### Disposing Resources

```csharp
// AmazonBedrockLanguageModel implements IDisposable
using var model = AmazonBedrockProvider.Claude35Sonnet("us-east-1");

var result = await model.GenerateAsync(options);
// Automatically disposes AWS client when exiting scope
```

## AWS Permissions

### Required IAM Permissions

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "bedrock:InvokeModel",
        "bedrock:InvokeModelWithResponseStream"
      ],
      "Resource": [
        "arn:aws:bedrock:*::foundation-model/anthropic.claude-*",
        "arn:aws:bedrock:*::foundation-model/amazon.titan-*",
        "arn:aws:bedrock:*::foundation-model/meta.llama*"
      ]
    }
  ]
}
```

### Model Access

Before using Bedrock models, you must:
1. Go to AWS Console > Bedrock > Model access
2. Request access to the models you want to use
3. Wait for approval (usually instant for most models)

## Supported Features by Model

| Feature | Claude | Titan | Llama |
|---------|--------|-------|-------|
| Basic Text Generation | ✅ | ✅ | ✅ |
| Streaming | ✅ | ❌ | ❌ |
| Tool Use | ✅ | ❌ | ❌ |
| Vision (Images) | ✅ | ❌ | ❌ |
| System Messages | ✅ | ✅ | ✅ |
| Temperature Control | ✅ | ✅ | ✅ |
| Top-P Sampling | ✅ | ✅ | ✅ |
| Stop Sequences | ✅ | ✅ | ❌ |

## Available Regions

Bedrock is available in the following AWS regions:
- `us-east-1` (US East - N. Virginia)
- `us-west-2` (US West - Oregon)
- `ap-southeast-1` (Asia Pacific - Singapore)
- `ap-northeast-1` (Asia Pacific - Tokyo)
- `eu-central-1` (Europe - Frankfurt)
- `eu-west-1` (Europe - Ireland)
- `eu-west-3` (Europe - Paris)

Check [AWS Bedrock documentation](https://docs.aws.amazon.com/bedrock/latest/userguide/what-is-bedrock.html) for the latest region availability.

## Model Comparison

### When to Use Claude
- Complex reasoning and analysis
- Code generation and debugging
- Tool/function calling
- Long-form content generation
- Vision tasks (image understanding)

### When to Use Titan
- Cost-effective text generation
- Simple Q&A and summarization
- English-language tasks
- Fast response times needed

### When to Use Llama
- Open-source model preference
- Instruction following
- Moderate complexity tasks
- Cost-conscious deployments

## Important Notes

1. **MaxTokens Required for Claude**: Claude models require the `MaxTokens` parameter. If not specified, defaults to 4096.

2. **Streaming Support**: Only Claude models support streaming. Titan and Llama will throw `NotSupportedException` if you call `StreamAsync`.

3. **Tool Use**: Only Claude models support tool/function calling.

4. **Model Access**: You must request access to models in the Bedrock console before using them.

5. **Pricing**: Bedrock pricing varies by model and region. Claude models are generally more expensive than Titan or Llama. Check [AWS Bedrock pricing](https://aws.amazon.com/bedrock/pricing/) for details.

6. **Rate Limits**: Bedrock has per-model rate limits. Monitor throttling (429 errors) and implement retry logic if needed.

7. **Model IDs**: Bedrock uses versioned model IDs (e.g., `anthropic.claude-3-5-sonnet-20241022-v2:0`). The provider includes convenience methods with the latest versions.

## Common Error Codes

| Error Code | Description | Solution |
|------------|-------------|----------|
| `AccessDeniedException` | Missing IAM permissions | Add required Bedrock permissions to IAM role/user |
| `ResourceNotFoundException` | Model not found or not enabled | Request model access in Bedrock console |
| `ThrottlingException` | Rate limit exceeded | Implement exponential backoff retry logic |
| `ValidationException` | Invalid request parameters | Check model requirements (e.g., MaxTokens) |
| `ModelTimeoutException` | Model invocation timeout | Increase timeout or reduce prompt complexity |
| `ServiceQuotaExceededException` | Service quota exceeded | Request quota increase in AWS Service Quotas |

## Performance Tips

1. **Use Streaming for Long Responses**: Claude streaming provides better UX for long-form content
2. **Choose the Right Model**: Claude 3.5 Sonnet for complex tasks, Haiku for speed, Titan for cost
3. **Set Appropriate MaxTokens**: Don't request more tokens than needed
4. **Implement Caching**: Cache responses for repeated queries
5. **Use IAM Roles**: More secure and eliminates credential rotation
6. **Monitor Costs**: Use AWS Cost Explorer to track Bedrock usage

## Links

- [AWS Bedrock Documentation](https://docs.aws.amazon.com/bedrock/)
- [Bedrock Model IDs](https://docs.aws.amazon.com/bedrock/latest/userguide/model-ids.html)
- [Bedrock Pricing](https://aws.amazon.com/bedrock/pricing/)
- [AI SDK Documentation](../../README.md)

## License

Apache-2.0
