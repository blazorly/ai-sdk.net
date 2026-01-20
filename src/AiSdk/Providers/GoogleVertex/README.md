# AiSdk.Providers.GoogleVertex

Google Vertex AI provider for AI SDK .NET. This package provides access to both **Google Gemini** and **Anthropic Claude** models through Google Cloud's Vertex AI platform.

## Features

- Multi-provider aggregator supporting both Gemini and Claude models
- Full ILanguageModel implementation
- Streaming support for both model types
- Function/tool calling support
- Google Cloud authentication (OAuth2 access token or service account)
- Automatic model type detection and routing
- Timeout enforcement
- Comprehensive error handling

## Installation

```bash
dotnet add package AiSdk.Providers.GoogleVertex
```

## Prerequisites

1. **Google Cloud Project**: You need a Google Cloud project with Vertex AI API enabled
2. **Authentication**: Either an OAuth2 access token or service account JSON
3. **Location**: A valid Google Cloud region (e.g., "us-central1", "europe-west1")

## Supported Models

### Google Gemini Models
- `gemini-1.5-pro` - Most capable Gemini model
- `gemini-1.5-flash` - Fast and efficient Gemini model
- `gemini-1.0-pro` - Previous generation Gemini model

### Anthropic Claude Models
- `claude-3-5-sonnet-20241022` - Latest Claude 3.5 Sonnet
- `claude-3-opus-20240229` - Most capable Claude 3 model
- `claude-3-sonnet-20240229` - Balanced Claude 3 model
- `claude-3-haiku-20240307` - Fast and compact Claude 3 model

## Usage

### Basic Usage with Access Token

```csharp
using AiSdk.Providers.GoogleVertex;

// Create a Gemini model
var geminiModel = GoogleVertexProvider.Gemini15Pro(
    projectId: "my-gcp-project",
    location: "us-central1",
    accessToken: "ya29...."
);

// Create a Claude model
var claudeModel = GoogleVertexProvider.Claude35Sonnet(
    projectId: "my-gcp-project",
    location: "us-central1",
    accessToken: "ya29...."
);

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is the capital of France?"
        }
    }
};

var result = await geminiModel.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Using Configuration Object

```csharp
var config = new GoogleVertexConfiguration
{
    ProjectId = "my-gcp-project",
    Location = "us-central1",
    AccessToken = "ya29....",
    TimeoutSeconds = 30
};

var model = GoogleVertexProvider.CreateModel("gemini-1.5-pro", config);
```

### Using Service Account (Alternative Authentication)

```csharp
var serviceAccountJson = File.ReadAllText("path/to/service-account.json");

var config = new GoogleVertexConfiguration
{
    ProjectId = "my-gcp-project",
    Location = "us-central1",
    ServiceAccountJson = serviceAccountJson
};

var model = GoogleVertexProvider.CreateModel("claude-3-5-sonnet-20241022", config);
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
            Content = "Write a short story about a robot."
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

### Function Calling

```csharp
var tools = new List<Tool>
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
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What's the weather in San Francisco?"
        }
    },
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls != null)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");
    }
}
```

### Custom Model ID

```csharp
// Automatically detects model type from ID
var customGemini = GoogleVertexProvider.CreateModel(
    "gemini-pro-vision",
    projectId: "my-project",
    location: "us-central1",
    accessToken: "ya29...."
);

var customClaude = GoogleVertexProvider.CreateModel(
    "claude-3-haiku-20240307",
    projectId: "my-project",
    location: "us-central1",
    accessToken: "ya29...."
);
```

## Configuration Options

### GoogleVertexConfiguration

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `ProjectId` | string | Yes | Google Cloud project ID |
| `Location` | string | Yes | Google Cloud region (e.g., "us-central1") |
| `AccessToken` | string | Conditional | OAuth2 access token (required if ServiceAccountJson not provided) |
| `ServiceAccountJson` | string | Conditional | Service account JSON content (required if AccessToken not provided) |
| `BaseUrl` | string | No | Custom base URL (defaults to https://{location}-aiplatform.googleapis.com/v1) |
| `TimeoutSeconds` | int? | No | Request timeout in seconds (default: 100) |

## Authentication

### Option 1: Access Token (Recommended for Development)

```csharp
// Get access token using gcloud CLI:
// gcloud auth print-access-token

var config = new GoogleVertexConfiguration
{
    ProjectId = "my-project",
    Location = "us-central1",
    AccessToken = "ya29...."
};
```

### Option 2: Service Account (Recommended for Production)

```csharp
var config = new GoogleVertexConfiguration
{
    ProjectId = "my-project",
    Location = "us-central1",
    ServiceAccountJson = File.ReadAllText("service-account.json")
};
```

## Model Detection and Routing

The provider automatically detects the model type based on the model ID:

- Models containing "gemini" → Routes to Gemini API endpoint
- Models containing "claude" → Routes to Claude API endpoint

```csharp
// Automatically uses Gemini endpoint
var gemini = GoogleVertexProvider.CreateModel("gemini-1.5-pro", config);

// Automatically uses Claude endpoint
var claude = GoogleVertexProvider.CreateModel("claude-3-5-sonnet-20241022", config);
```

## API Endpoints

### Gemini Models
- Generate: `projects/{projectId}/locations/{location}/publishers/google/models/{model}:generateContent`
- Stream: `projects/{projectId}/locations/{location}/publishers/google/models/{model}:streamGenerateContent`

### Claude Models
- Generate/Stream: `projects/{projectId}/locations/{location}/publishers/anthropic/models/{model}:streamRawPredict`

## Error Handling

```csharp
using AiSdk.Providers.GoogleVertex.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (GoogleVertexException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Model Type: {ex.ModelType}");
    Console.WriteLine($"Error Status: {ex.ErrorStatus}");
}
```

## Advanced Usage

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(60)
};

var model = GoogleVertexProvider.Gemini15Pro(
    projectId: "my-project",
    location: "us-central1",
    accessToken: "ya29....",
    httpClient: httpClient
);
```

### Multiple Regions

```csharp
var usModel = GoogleVertexProvider.CreateModel(
    "gemini-1.5-pro",
    projectId: "my-project",
    location: "us-central1",
    accessToken: token
);

var euModel = GoogleVertexProvider.CreateModel(
    "gemini-1.5-pro",
    projectId: "my-project",
    location: "europe-west1",
    accessToken: token
);
```

## Supported Locations

Common Google Cloud regions for Vertex AI:
- `us-central1` (Iowa)
- `us-east1` (South Carolina)
- `us-west1` (Oregon)
- `europe-west1` (Belgium)
- `europe-west4` (Netherlands)
- `asia-northeast1` (Tokyo)
- `asia-southeast1` (Singapore)

Check [Google Cloud documentation](https://cloud.google.com/vertex-ai/docs/general/locations) for the latest list.

## Differences from Direct Provider SDKs

### vs. AiSdk.Providers.Google
- Requires Google Cloud project and authentication
- Uses Vertex AI endpoints instead of AI Studio
- Access to both Gemini and Claude models
- Enterprise features and SLAs available

### vs. AiSdk.Providers.Anthropic
- Claude models accessed through Google Cloud
- Uses Vertex AI billing and quotas
- May have different model versions available
- Requires Google Cloud authentication

## Best Practices

1. **Cache Access Tokens**: Access tokens expire after 1 hour, implement token refresh logic
2. **Use Service Accounts in Production**: More secure than access tokens for server applications
3. **Set Appropriate Timeouts**: Adjust timeout based on expected response time
4. **Handle Errors Gracefully**: Implement retry logic for transient failures
5. **Monitor Usage**: Use Google Cloud Console to monitor API usage and costs

## Troubleshooting

### Authentication Errors
- Ensure Vertex AI API is enabled in your project
- Verify access token is valid and not expired
- Check service account has necessary permissions

### Model Not Found
- Verify model ID is correct
- Check if model is available in your region
- Ensure you have access to the model in your project

### Timeout Errors
- Increase `TimeoutSeconds` in configuration
- Check network connectivity to Google Cloud
- Consider using streaming for long-running requests

## License

This package is part of the AI SDK .NET project.

## Contributing

Contributions are welcome! Please see the main repository for contribution guidelines.

## Related Packages

- `AiSdk.Abstractions` - Core abstractions and interfaces
- `AiSdk.Providers.Google` - Direct Google AI Studio provider
- `AiSdk.Providers.Anthropic` - Direct Anthropic provider
- `AiSdk.Providers.OpenAI` - OpenAI provider
- `AiSdk.Providers.Azure` - Azure OpenAI provider
