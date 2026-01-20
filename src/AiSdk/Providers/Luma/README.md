# AiSdk.Providers.Luma

Luma AI provider for AI SDK .NET - a placeholder structure for integrating Luma AI's Dream Machine video generation capabilities with the unified AI SDK interface.

## Important Notice

**This provider is currently a placeholder structure for future video generation features.**

Luma AI specializes in:
- **Text-to-Video Generation** (Dream Machine)
- **Image-to-Video Generation** (Dream Machine)
- **3D Asset Generation** (Genie)

Luma AI is **NOT a language model provider** like OpenAI or Anthropic. This implementation provides a foundational structure that can be extended in the future when the AI SDK adds support for video and image generation interfaces (e.g., `IVideoModel`, `IImageModel`).

## Current Status

| Feature | Status |
|---------|--------|
| Text-to-Video | ⏸ Planned - awaiting SDK video generation interface |
| Image-to-Video | ⏸ Planned - awaiting SDK video generation interface |
| 3D Generation (Genie) | ⏸ Planned - awaiting SDK 3D generation interface |
| Chat Completions | ❌ Not supported (Luma is not an LLM) |
| Streaming | ❌ Not supported (Luma is not an LLM) |

## Installation

```bash
dotnet add package AiSdk
```

The Luma provider is included in the consolidated AiSdk package.

## About Luma AI

Luma AI is a cutting-edge video generation company known for:

### Dream Machine
Dream Machine is Luma's flagship text-to-video and image-to-video generation model that creates high-quality, realistic videos from text prompts or images.

**Key Features:**
- High-quality video generation from text descriptions
- Image-to-video conversion
- Smooth motion and coherent scenes
- Fast generation times
- Support for various aspect ratios and durations

**Use Cases:**
- Marketing and advertising content
- Social media videos
- Product demonstrations
- Creative storytelling
- Concept visualization
- Educational content

### Genie
Genie is Luma's 3D asset generation model that creates 3D objects from text or images.

## Future Implementation Roadmap

When the AI SDK adds video generation support, this provider will implement:

1. **Video Generation Interface** (`IVideoModel`)
   - Text-to-video generation
   - Image-to-video generation
   - Configurable duration, aspect ratio, and quality
   - Generation status polling
   - Video URL retrieval

2. **3D Generation Interface** (`I3DModel`)
   - Text-to-3D generation
   - Image-to-3D generation
   - 3D asset format support (GLB, FBX, etc.)

3. **Extended Configuration**
   - Video generation parameters
   - Quality settings
   - Duration controls
   - Aspect ratio presets

## Configuration Structure

The basic configuration structure is already in place:

```csharp
using AiSdk.Providers.Luma;

var config = new LumaConfiguration
{
    ApiKey = "luma_...",                                  // Required
    BaseUrl = "https://api.lumalabs.ai/v1",              // Optional (default shown)
    TimeoutSeconds = 120                                  // Optional (default: 100)
};

var provider = new LumaProvider(config);
```

### Environment Variables

```csharp
var config = new LumaConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("LUMA_API_KEY")!
};
```

## Placeholder API

Currently, attempting to use the provider will throw a descriptive exception:

```csharp
var provider = new LumaProvider(config);
var model = provider.DreamMachine();

// This will throw LumaException with a clear message
try
{
    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, "Generate a video")
        }
    };

    var result = await model.GenerateAsync(options);
}
catch (LumaException ex)
{
    Console.WriteLine(ex.Message);
    // Output: "Luma AI is a video generation service. Traditional chat completion
    //          is not supported. This provider is a placeholder structure for
    //          future video generation features."
}
```

## Future Usage Examples

These examples show how the provider might be used once video generation interfaces are implemented:

### Text-to-Video Generation (Future)

```csharp
// Future implementation example
var provider = new LumaProvider(config);
var model = provider.DreamMachine();

var videoOptions = new VideoGenerationOptions
{
    Prompt = "A serene sunset over a mountain lake with wildlife",
    Duration = 5,  // seconds
    AspectRatio = "16:9",
    Quality = "high"
};

var generation = await model.GenerateVideoAsync(videoOptions);

// Poll for completion
while (generation.Status == GenerationStatus.Processing)
{
    await Task.Delay(1000);
    generation = await model.GetGenerationStatusAsync(generation.Id);
}

if (generation.Status == GenerationStatus.Completed)
{
    Console.WriteLine($"Video URL: {generation.VideoUrl}");
    // Download or display the generated video
}
```

### Image-to-Video Generation (Future)

```csharp
// Future implementation example
var videoOptions = new VideoGenerationOptions
{
    Prompt = "Animate this scene with gentle camera movement",
    SourceImageUrl = "https://example.com/image.jpg",
    Duration = 3,
    AspectRatio = "1:1"
};

var generation = await model.GenerateVideoAsync(videoOptions);
```

### 3D Asset Generation with Genie (Future)

```csharp
// Future implementation example
var genieModel = provider.Genie();

var assetOptions = new ThreeDGenerationOptions
{
    Prompt = "A detailed medieval sword with ornate handle",
    Format = "glb"
};

var generation = await genieModel.Generate3DAsync(assetOptions);
```

## Why Luma AI?

Luma AI stands out in the video generation space:

- **State-of-the-Art Quality**: Dream Machine produces remarkably realistic videos
- **Fast Generation**: Competitive generation times compared to other solutions
- **Versatile Input**: Both text and image-to-video capabilities
- **Professional Output**: Suitable for commercial use and high-quality content
- **Innovative Technology**: Built on cutting-edge video diffusion models
- **Active Development**: Continuously improving models and capabilities

Perfect for:
- Content creators and marketers
- Product designers and demonstrators
- Educators creating visual content
- Social media managers
- Game developers (3D assets)
- Film and animation pre-visualization
- Rapid prototyping of visual concepts

## API Structure

The Luma AI API (when implemented) will likely follow this pattern:

### Video Generation Request

```json
{
  "prompt": "A serene sunset over mountains",
  "image_url": "https://example.com/image.jpg",  // Optional for image-to-video
  "aspect_ratio": "16:9",
  "duration": 5,
  "quality": "high"
}
```

### Video Generation Response

```json
{
  "id": "gen_abc123",
  "status": "processing",
  "created_at": 1234567890,
  "estimated_completion_time": 30
}
```

### Completed Generation

```json
{
  "id": "gen_abc123",
  "status": "completed",
  "video_url": "https://luma-cdn.com/videos/abc123.mp4",
  "thumbnail_url": "https://luma-cdn.com/thumbs/abc123.jpg",
  "duration": 5.2,
  "resolution": "1920x1080"
}
```

## Error Handling

The error handling structure is already implemented:

```csharp
using AiSdk.Providers.Luma.Exceptions;

try
{
    // Future video generation call
    var generation = await model.GenerateVideoAsync(options);
}
catch (LumaException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (LumaException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (LumaException ex) when (ex.StatusCode == 501)
{
    Console.WriteLine("Feature not yet implemented");
}
catch (LumaException ex)
{
    Console.WriteLine($"Luma error: {ex.Message} (Status: {ex.StatusCode})");
}
catch (AiSdkException ex)
{
    Console.WriteLine($"SDK error: {ex.Message}");
}
```

## Getting an API Key

1. Visit [Luma Labs](https://lumalabs.ai)
2. Sign up for an account
3. Access the Dream Machine platform
4. Navigate to API settings
5. Create a new API key
6. Copy the key (starts with `luma_`)

Note: As of early 2026, Luma AI may still be in beta or limited access. Check their website for current API availability.

## Configuration with ASP.NET Core

### appsettings.json (Future)

```json
{
  "AiSdk": {
    "Providers": {
      "luma": {
        "ApiKey": "luma_...",
        "DefaultModel": "dream-machine-1.0",
        "TimeoutSeconds": 120,
        "Enabled": true
      }
    }
  }
}
```

### Startup Registration (Future)

```csharp
using AiSdk.AspNetCore;

builder.Services.AddAiSdk(options =>
{
    options.DefaultProvider = "luma";
    options.Providers.Add("luma", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:luma:ApiKey"]!,
        DefaultModel = "dream-machine-1.0"
    });
});
```

## Model Comparison

| Model | Type | Purpose | Input | Output |
|-------|------|---------|-------|--------|
| Dream Machine 1.0 | Video | Text/Image-to-Video | Text prompt or Image | Video (MP4) |
| Genie | 3D | Text/Image-to-3D | Text prompt or Image | 3D Asset (GLB, etc.) |

## Development Notes

This provider is structured to match the existing AI SDK patterns:

1. **Configuration**: Standard `LumaConfiguration` with API key, base URL, and timeout
2. **Provider Factory**: `LumaProvider` with model creation methods
3. **Model Implementation**: `LumaChatLanguageModel` implementing `ILanguageModel` (placeholder)
4. **Request/Response Models**: Structured in `/Models` directory
5. **Exception Handling**: Custom `LumaException` with marker pattern
6. **Documentation**: Comprehensive README following SDK conventions

## Contributing

When video generation interfaces are added to the AI SDK:

1. Implement `IVideoModel` interface
2. Add video generation methods to `LumaProvider`
3. Create proper request/response models for video generation
4. Implement status polling and URL retrieval
5. Add comprehensive tests
6. Update documentation with real examples

## Links

- [Luma Labs](https://lumalabs.ai)
- [Dream Machine Platform](https://lumalabs.ai/dream-machine)
- [Luma AI Documentation](https://docs.lumalabs.ai) (when available)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## Technical Specifications

### Supported Parameters (Future)

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `prompt` | string | Text description of the video to generate | Required |
| `image_url` | string | Source image for image-to-video | Optional |
| `duration` | int | Video duration in seconds (1-10) | 5 |
| `aspect_ratio` | string | Video aspect ratio (16:9, 1:1, 9:16) | 16:9 |
| `quality` | string | Generation quality (standard, high) | high |
| `frame_rate` | int | Frames per second | 24 |

### API Endpoints (Future)

- `POST /v1/generations` - Create new video generation
- `GET /v1/generations/{id}` - Get generation status
- `DELETE /v1/generations/{id}` - Cancel generation
- `GET /v1/generations` - List user's generations

## License

Apache-2.0
