# Hugging Face Provider

This provider enables integration with Hugging Face Inference API for the AI SDK .NET project.

## Features

- Full ILanguageModel implementation
- Support for streaming and non-streaming responses
- Compatible with thousands of models on Hugging Face Hub
- Built-in factory methods for popular models

## Installation

The Hugging Face provider is included in the AiSdk package.

## Configuration

```csharp
using AiSdk.Providers.HuggingFace;

var config = new HuggingFaceConfiguration
{
    ApiKey = "your-huggingface-api-key",
    BaseUrl = "https://api-inference.huggingface.co/models/", // Optional, this is the default
    TimeoutSeconds = 120 // Optional
};
```

## Usage

### Using Factory Methods for Popular Models

```csharp
using AiSdk.Providers.HuggingFace;

// Llama 2 models
var llama70B = HuggingFaceProvider.Llama2_70B_Chat("your-api-key");
var llama13B = HuggingFaceProvider.Llama2_13B_Chat("your-api-key");
var llama7B = HuggingFaceProvider.Llama2_7B_Chat("your-api-key");

// Mistral models
var mistral7B = HuggingFaceProvider.Mistral7B_Instruct("your-api-key");
var mixtral8x7B = HuggingFaceProvider.Mixtral8x7B_Instruct("your-api-key");

// Other models
var zephyr = HuggingFaceProvider.Zephyr7B_Beta("your-api-key");
var codeLlama = HuggingFaceProvider.CodeLlama34B_Instruct("your-api-key");
```

### Using Custom Model IDs

```csharp
using AiSdk.Providers.HuggingFace;

// Use any model from Hugging Face Hub
var model = HuggingFaceProvider.ChatModel("organization/model-name", "your-api-key");

// Examples:
var customModel1 = HuggingFaceProvider.ChatModel("google/flan-t5-xxl", "your-api-key");
var customModel2 = HuggingFaceProvider.ChatModel("tiiuae/falcon-40b-instruct", "your-api-key");
```

### Generate Text (Non-Streaming)

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.HuggingFace;

var model = HuggingFaceProvider.Llama2_70B_Chat("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message { Role = MessageRole.System, Content = "You are a helpful assistant." },
        new Message { Role = MessageRole.User, Content = "What is machine learning?" }
    },
    MaxTokens = 500,
    Temperature = 0.7
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Stream Text Generation

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.HuggingFace;

var model = HuggingFaceProvider.Mistral7B_Instruct("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message { Role = MessageRole.User, Content = "Write a short story about AI." }
    },
    MaxTokens = 1000,
    Temperature = 0.8
};

await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

## Supported Models

The Hugging Face Inference API supports thousands of models. Some popular categories include:

### Chat Models
- **Llama 2**: Meta's family of open-source models (7B, 13B, 70B)
- **Mistral**: Mistral AI's high-performance models
- **Mixtral**: Mixture of Experts models
- **Zephyr**: Fine-tuned models optimized for chat

### Code Models
- **CodeLlama**: Meta's code-specialized models
- **StarCoder**: Code generation models
- **WizardCoder**: Instruction-tuned code models

### Domain-Specific Models
- **Falcon**: Technology Innovation Institute's models
- **MPT**: MosaicML's pretrained transformers
- **Bloom**: Multilingual models

## API Endpoint

The default endpoint is:
```
https://api-inference.huggingface.co/models/{model-id}
```

You can override this in the configuration if needed.

## Error Handling

```csharp
using AiSdk.Providers.HuggingFace.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (HuggingFaceException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Type: {ex.ErrorType}");
}
```

## Model ID Format

Model IDs on Hugging Face follow the format: `organization/model-name`

Examples:
- `meta-llama/Llama-2-70b-chat-hf`
- `mistralai/Mistral-7B-Instruct-v0.2`
- `HuggingFaceH4/zephyr-7b-beta`
- `codellama/CodeLlama-34b-Instruct-hf`

## Authentication

To use the Hugging Face Inference API, you need an API key:

1. Sign up at https://huggingface.co
2. Go to Settings > Access Tokens
3. Create a new token with read permissions
4. Use the token as your API key

## Limitations

- The free Inference API has rate limits
- Some models require Pro subscription or Inference Endpoints
- Tool calling is not natively supported by the Inference API
- Token usage metrics may not be available for all models

## Additional Resources

- [Hugging Face Inference API Documentation](https://huggingface.co/docs/inference-providers)
- [Model Hub](https://huggingface.co/models)
- [Pricing Information](https://huggingface.co/pricing)
