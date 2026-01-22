# Z.AI Example

This example demonstrates how to use the Z.AI provider with the AI SDK for .NET.

## Prerequisites

- .NET 10.0 SDK or later
- A Z.AI API key (get one from [z.ai](https://z.ai))

## Setup

1. Set your Z.AI API key as an environment variable:

**Windows (PowerShell):**
```powershell
$env:ZAI_API_KEY = "your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set ZAI_API_KEY=your-api-key-here
```

**Linux/macOS:**
```bash
export ZAI_API_KEY=your-api-key-here
```

2. Run the example:
```bash
dotnet run
```

## Examples Included

This example demonstrates:

### 1. Basic Chat with GLM-4.7
Simple conversation with the latest Z.AI model for general-purpose tasks.

### 2. Code Generation with CodeGeeX-4
Using the specialized CodeGeeX-4 model for code generation tasks with better code quality and understanding.

### 3. Streaming Response
Real-time streaming of responses for a better user experience.

### 4. Extended Context (GLM-4-32B-128K)
Using the extended context model to process long documents (up to 128K tokens).

### 5. Tool Calling
Demonstrating function calling capabilities for integrating external tools.

## Available Models

The Z.AI provider supports:

- **GLM-4.7** - Latest general-purpose model
- **GLM-4.6** - Previous generation model
- **GLM-4-32B-128K** - Extended context model (128K tokens)
- **CodeGeeX-4** - Specialized code generation model

## Key Features Demonstrated

- ✅ Basic text generation
- ✅ Code generation with specialized models
- ✅ Real-time streaming
- ✅ Long document processing
- ✅ Tool/function calling
- ✅ Token usage tracking
- ✅ Temperature control
- ✅ Max tokens configuration

## Learn More

- [Z.AI Documentation](https://docs.z.ai)
- [Z.AI Models Guide](https://docs.z.ai/guides/llm)
- [API Reference](https://docs.z.ai/api-reference)
