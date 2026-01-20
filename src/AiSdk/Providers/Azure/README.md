# AiSdk.Providers.Azure

Azure OpenAI Service provider for the AI SDK for .NET.

## Overview

This package provides integration with Azure OpenAI Service, allowing you to use Azure-hosted OpenAI models through the unified AI SDK interface.

## Installation

```bash
dotnet add package AiSdk.Providers.Azure
```

## Quick Start

### Basic Usage with API Key

```csharp
using AiSdk.Providers.Azure;
using AiSdk.Abstractions;

// Create a chat model using API key authentication
var model = AzureOpenAIProvider.CreateChatModel(
    deploymentName: "gpt-4-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key"
);

// Generate text
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is Azure OpenAI?"
        }
    }
});

Console.WriteLine(result.Text);
```

### Using Azure AD Authentication

```csharp
var model = AzureOpenAIProvider.CreateChatModelWithAzureAd(
    deploymentName: "gpt-4-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    azureAdToken: "your-azure-ad-token"
);
```

### Streaming Responses

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a short story about AI"
        }
    }
};

await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

### Using Configuration Object

```csharp
var config = new AzureOpenAIConfiguration
{
    Endpoint = "https://your-resource.openai.azure.com",
    DeploymentName = "gpt-4-deployment",
    ApiKey = "your-api-key",
    ApiVersion = "2024-02-15-preview"
};

var model = AzureOpenAIProvider.CreateChatModel(config);
```

### Convenience Methods for Common Models

```csharp
// GPT-4
var gpt4 = AzureOpenAIProvider.GPT4(
    deploymentName: "gpt-4-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key"
);

// GPT-4 Turbo
var gpt4Turbo = AzureOpenAIProvider.GPT4Turbo(
    deploymentName: "gpt-4-turbo-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key"
);

// GPT-3.5 Turbo
var gpt35 = AzureOpenAIProvider.GPT35Turbo(
    deploymentName: "gpt-35-turbo-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key"
);
```

## Configuration

### AzureOpenAIConfiguration Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `Endpoint` | `string` | Yes | Azure OpenAI endpoint URL (e.g., `https://your-resource.openai.azure.com`) |
| `DeploymentName` | `string` | Yes | The deployment name configured in Azure OpenAI |
| `ApiKey` | `string` | Conditional | API key for authentication (required if `AzureAdToken` is not provided) |
| `AzureAdToken` | `string` | Conditional | Azure AD token for authentication (alternative to `ApiKey`) |
| `ApiVersion` | `string` | No | API version (defaults to `2024-02-15-preview`) |

### API Versions

The provider supports Azure OpenAI API versions including:
- `2024-02-15-preview` (default)
- `2023-12-01-preview`
- Other versions as released by Azure

## Features

### Supported Capabilities

- Text generation (non-streaming)
- Streaming text generation
- Tool/Function calling
- Multi-turn conversations
- System messages
- Temperature and top-p sampling
- Max tokens control
- Stop sequences
- Vision models (image URLs)

### Error Handling

```csharp
using AiSdk.Providers.Azure.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (AzureOpenAIException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
```

## Advanced Usage

### Function Calling / Tool Use

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What's the weather in Seattle?"
        }
    },
    Tools = new List<Tool>
    {
        new Tool
        {
            Name = "get_weather",
            Description = "Get the current weather for a location",
            Parameters = JsonDocument.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""location"": {
                        ""type"": ""string"",
                        ""description"": ""The city and state""
                    }
                },
                ""required"": [""location""]
            }")
        }
    }
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");
    }
}
```

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(120)
};

var model = AzureOpenAIProvider.CreateChatModel(
    deploymentName: "gpt-4-deployment",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key",
    httpClient: httpClient
);
```

## Azure OpenAI Service Setup

1. Create an Azure OpenAI resource in the Azure portal
2. Deploy a model (e.g., GPT-4, GPT-3.5-turbo) and note the deployment name
3. Get your endpoint URL and API key from the Azure portal
4. Use these values in your configuration

### Finding Your Configuration Values

- **Endpoint**: Found in the Azure portal under "Keys and Endpoint" for your Azure OpenAI resource
- **DeploymentName**: The name you gave when deploying a model in Azure OpenAI Studio
- **ApiKey**: Found in the Azure portal under "Keys and Endpoint"

## Differences from OpenAI Provider

The Azure provider is similar to the OpenAI provider but with key differences:

1. **Endpoint Structure**: Azure uses deployment-based endpoints instead of model names in the request body
2. **Authentication**: Supports both API key and Azure AD authentication
3. **API Versioning**: Requires explicit API version parameter
4. **Deployment Names**: Uses Azure deployment names instead of model IDs

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please see the main repository for contribution guidelines.
