# Gateway Provider Porting Status: TypeScript → .NET C#

## Overview
This document tracks the porting of the Vercel AI Gateway provider from TypeScript to .NET C#.

## TypeScript Source Files Analysis

### Core Provider Files
1. `src/index.ts` - Main exports
2. `src/gateway-provider.ts` - Core provider implementation (285 lines)
3. `src/gateway-provider-options.ts` - Provider options schema (67 lines)
4. `src/gateway-config.ts` - Configuration interface (8 lines)

### Model Implementation Files
5. `src/gateway-language-model.ts` - Language model implementation (213 lines)
6. `src/gateway-embedding-model.ts` - Embedding model implementation (110 lines)
7. `src/gateway-image-model.ts` - Image model implementation (146 lines)

### Model Settings/IDs Files
8. `src/gateway-language-model-settings.ts` - Language model ID types (158 lines)
9. `src/gateway-embedding-model-settings.ts` - Embedding model ID types (23 lines)
10. `src/gateway-image-model-settings.ts` - Image model ID types (11 lines)
11. `src/gateway-model-entry.ts` - Model entry interfaces (59 lines)
12. `src/gateway-fetch-metadata.ts` - Metadata fetching (128 lines)

### Error Handling Files
13. `src/errors/index.ts` - Error exports (17 lines)
14. `src/errors/gateway-error.ts` - Base error class (44 lines)
15. `src/errors/gateway-authentication-error.ts` - Auth error (80 lines)
16. `src/errors/gateway-invalid-request-error.ts` - Invalid request error (32 lines)
17. `src/errors/gateway-rate-limit-error.ts` - Rate limit error (32 lines)
18. `src/errors/gateway-model-not-found-error.ts` - Model not found error (46 lines)
19. `src/errors/gateway-internal-server-error.ts` - Internal server error (32 lines)
20. `src/errors/gateway-response-error.ts` - Response error (41 lines)
21. `src/errors/create-gateway-error.ts` - Error creation factory (100 lines)
22. `src/errors/as-gateway-error.ts` - Error conversion (34 lines)
23. `src/errors/parse-auth-method.ts` - Auth method parsing (24 lines)
24. `src/errors/extract-api-call-response.ts` - Response extraction (16 lines)

### Tools Files
25. `src/gateway-tools.ts` - Tools exports (16 lines)
26. `src/tool/perplexity-search.ts` - Perplexity search tool (295 lines)

### Utility Files
27. `src/vercel-environment.ts` - Vercel environment utilities (7 lines)
28. `src/version.ts` - Version constant (7 lines)

**Total: 28 TypeScript files, ~2,000 lines of code**

---

## Porting Status: Completed ✅ (Build Successful)

### 1. Error Handling Classes (7/7 files ported) ✅ Build Verified

| TypeScript File | C# File | Status | Lines |
|----------------|-----------|--------|-------|
| `errors/gateway-error.ts` | `Exceptions/GatewayError.cs` | ✅ Complete | 52 |
| `errors/gateway-authentication-error.ts` | `Exceptions/GatewayAuthenticationError.cs` | ✅ Complete | 83 |
| `errors/gateway-invalid-request-error.ts` | `Exceptions/GatewayInvalidRequestError.cs` | ✅ Complete | 40 |
| `errors/gateway-rate-limit-error.ts` | `Exceptions/GatewayRateLimitError.cs` | ✅ Complete | 40 |
| `errors/gateway-model-not-found-error.ts` | `Exceptions/GatewayModelNotFoundError.cs` | ✅ Complete | 48 |
| `errors/gateway-internal-server-error.ts` | `Exceptions/GatewayInternalServerError.cs` | ✅ Complete | 40 |
| `errors/gateway-response-error.ts` | `Exceptions/GatewayResponseError.cs` | ✅ Complete | 51 |

**Error Features Ported:**
- ✅ Base `GatewayError` abstract class with marker pattern
- ✅ All error types: `GatewayAuthenticationError`, `GatewayInvalidRequestError`, `GatewayRateLimitError`, `GatewayModelNotFoundError`, `GatewayInternalServerError`, `GatewayResponseError`
- ✅ `CreateContextualError()` method for authentication error with detailed messages
- ✅ Static `IsInstance()` methods for type checking
- ✅ Marker-based type identification pattern
- ✅ Proper inheritance from `ApiCallError` base class

### 2. Configuration (1/2 files ported) ✅ Build Verified

| TypeScript File | C# File | Status | Lines |
|----------------|-----------|--------|-------|
| `gateway-config.ts` | `GatewayConfiguration.cs` | ✅ Complete | 28 |
| `gateway-provider-options.ts` | *Not yet created* | ⏳ Pending | - |

**Configuration Features Ported:**
- ✅ `BaseUrl` with default "https://ai-gateway.vercel.sh/v3/ai"
- ✅ `ApiKey` for authentication
- ✅ `Headers` for custom headers
- ✅ `TimeoutSeconds` for request timeout
- ✅ `MetadataCacheRefreshMillis` with default 5 minutes
- ✅ `Internal` settings for testing with `CurrentDate` function

### 3. Model IDs (3/3 files ported) ✅ Build Verified (CS1591 warnings suppressed)

| TypeScript File | C# File | Status | Lines |
|----------------|-----------|--------|-------|
| `gateway-language-model-settings.ts` | `Models/GatewayLanguageModelIds.cs` | ✅ Complete | 150 |
| `gateway-embedding-model-settings.ts` | `Models/GatewayEmbeddingModelIds.cs` | ✅ Complete | 22 |
| `gateway-image-model-settings.ts` | `Models/GatewayImageModelIds.cs` | ✅ Complete | 16 |

**Model IDs Ported:**
- ✅ ~100 language model IDs (Alibaba, Amazon, Anthropic, OpenAI, Google, Meta, DeepSeek, Mistral, Cohere, Perplexity, xAI, Zai, etc.)
- ✅ 19 embedding model IDs (Qwen, Amazon, Cohere, Google, Mistral, OpenAI, Voyage)
- ✅ 8 image model IDs (Flux, Imagen)

---

## Porting Status: Pending ⏳

### 4. Core Provider Implementation (0/4 files)

| TypeScript File | C# File | Status | Complexity |
|----------------|-----------|--------|------------|
| `gateway-provider.ts` | *GatewayProvider.cs* | ⏳ Not started | High |
| `index.ts` | *Gateway.cs* (or namespace) | ⏳ Not started | Low |
| `gateway-language-model.ts` | *GatewayChatLanguageModel.cs* | ⏳ Not started | High |
| `gateway-embedding-model.ts` | *GatewayEmbeddingModel.cs* | ⏳ Not started | Medium |
| `gateway-image-model.ts` | *GatewayImageModel.cs* | ⏳ Not started | Medium |

**Required GatewayProvider Features:**
- ⏳ `createGatewayProvider(options)` factory function
- ⏳ Default `gateway` provider instance
- ⏳ `languageModel(modelId)` method
- ⏳ `embeddingModel(modelId)` method
- ⏳ `imageModel(modelId)` method
- ⏳ `getAvailableModels()` method (metadata caching)
- ⏳ `getCredits()` method
- ⏳ `tools` property with `gatewayTools`
- ⏳ `getGatewayAuthToken()` function (API key vs OIDC)
- ⏳ AI Gateway protocol version: "0.0.1"
- ⏳ Default base URL: "https://ai-gateway.vercel.sh/v3/ai"

**Required GatewayLanguageModel Features:**
- ⏳ Implements `ILanguageModel` interface
- ⏳ `SpecificationVersion = "v3"`
- ⏳ `Provider = "gateway"`
- ⏳ File part encoding (base64 data URLs)
- ⏳ `doGenerate()` with streaming and non-streaming
- ⏳ `doStream()` with SSE handling
- ⏳ Model config headers: `ai-language-model-specification-version`, `ai-language-model-id`, `ai-language-model-streaming`
- ⏳ O11y headers for observability (deployment ID, environment, region, request ID)
- ⏳ Raw chunk filtering based on `includeRawChunks`

### 5. Error Factory & Conversion (0/4 files)

| TypeScript File | C# File | Status | Complexity |
|----------------|-----------|--------|------------|
| `create-gateway-error.ts` | *GatewayErrorFactory.cs* | ⏳ Not started | High |
| `as-gateway-error.ts` | *GatewayErrorExtensions.cs* | ⏳ Not started | Medium |
| `parse-auth-method.ts` | *GatewayAuthMethodParser.cs* | ⏳ Not started | Low |
| `extract-api-call-response.ts` | *GatewayResponseExtractor.cs* | ⏳ Not started | Low |

**Required Features:**
- ⏳ `createGatewayErrorFromResponse()` factory with error type switching
- ⏳ `asGatewayError()` conversion from unknown errors
- ⏳ `parseAuthMethod()` to extract "api-key" or "oidc" from headers
- ⏳ `extractApiCallResponse()` from `ApiCallError`

### 6. Metadata & Model Entry (0/2 files)

| TypeScript File | C# File | Status | Complexity |
|----------------|-----------|--------|------------|
| `gateway-fetch-metadata.ts` | *GatewayMetadataFetcher.cs* | ⏳ Not started | Medium |
| `gateway-model-entry.ts` | *Models/GatewayModelEntry.cs* | ⏳ Not started | Low |

**Required Features:**
- ⏳ `GatewayFetchMetadata` class
- ⏳ `getAvailableModels()` method
- ⏳ `getCredits()` method
- ⏳ `GatewayLanguageModelEntry` interface
- ⏳ `GatewayLanguageModelSpecification` record
- ⏳ Pricing information structure
- ⏳ Model type: "language", "embedding", or "image"

### 7. Tools (0/2 files)

| TypeScript File | C# File | Status | Complexity |
|----------------|-----------|--------|------------|
| `gateway-tools.ts` | *GatewayTools.cs* | ⏳ Not started | Low |
| `tool/perplexity-search.ts` | *Tools/PerplexitySearchTool.cs* | ⏳ Not started | High |

**Required Features:**
- ⏳ `gatewayTools` static class with `perplexitySearch`
- ⏳ `PerplexitySearchConfig` record
- ⏳ `PerplexitySearchInput` record
- ⏳ `PerplexitySearchResult` record
- ⏳ `PerplexitySearchResponse` record
- ⏳ All search parameters: max_results, max_tokens_per_page, max_tokens, country, search_domain_filter, search_language_filter, date filters, search_recency_filter

### 8. Utilities (0/2 files)

| TypeScript File | C# File | Status | Complexity |
|----------------|-----------|--------|------------|
| `vercel-environment.ts` | *VercelEnvironment.cs* | ⏳ Not started | High |
| `version.ts` | *Version.cs* | ⏳ Not started | Low |

**Required Features:**
- ⏳ `getVercelOidcToken()` function (from `@vercel/oidc`)
- ⏳ `getVercelRequestId()` function
- ⏳ `getContext().headers['x-vercel-id']` access
- ⏳ `VERSION` constant with build-time injection

---

## Summary

### Completed (25%) - Build Status: ✅ SUCCESS
- ✅ All error classes (7 files)
- ✅ Configuration (1 file)
- ✅ Model IDs (3 files)
- ✅ Directory structure created
- **Total: ~350 lines of C# code**

### Remaining (75%)
- ⏳ Core provider implementation (5 files, ~600 lines)
- ⏳ Error factory & conversion (4 files, ~170 lines)
- ⏳ Metadata & model entry (2 files, ~180 lines)
- ⏳ Tools (2 files, ~310 lines)
- ⏳ Utilities (2 files, ~15 lines)
- **Estimated remaining: ~1,275 lines of C# code**

### Key Challenges for Completion
1. **Vercel OIDC Integration**: The TypeScript version uses `@vercel/oidc` package which is Node.js-specific. The .NET version will need a different approach or may omit this feature initially.

2. **Streaming SSE Handling**: The TypeScript version uses Web API `TransformStream` and `EventSource`. .NET will need `IAsyncEnumerable` and SSE parsing.

3. **Tool System**: The Perplexity search tool is complex with many optional parameters and detailed validation.

4. **Metadata Caching**: The TypeScript version has sophisticated caching with refresh intervals. This needs careful async implementation in C#.

5. **Environment Variables**: Need to check environment variable access patterns in .NET (e.g., `Environment.GetEnvironmentVariable`).

---

## Recommended Next Steps

1. Complete `GatewayProvider.cs` - the core factory that creates models
2. Implement `GatewayChatLanguageModel.cs` - main language model
3. Implement `GatewayEmbeddingModel.cs` - embedding support
4. Implement `GatewayImageModel.cs` - image generation
5. Create error factory and conversion utilities
6. Implement metadata fetching with caching
7. Add Perplexity search tool
8. Add comprehensive tests

---

## File Structure

```
ai-sdk.net/src/AiSdk/Providers/Gateway/
├── GatewayConfiguration.cs              ✅ Created
├── GatewayProvider.cs                   ⏳ TODO
├── Gateway.cs                          ⏳ TODO (or namespace)
├── Models/
│   ├── GatewayLanguageModelIds.cs        ✅ Created
│   ├── GatewayEmbeddingModelIds.cs      ✅ Created
│   ├── GatewayImageModelIds.cs          ✅ Created
│   ├── GatewayModelEntry.cs             ⏳ TODO
│   └── GatewayMetadata.cs               ⏳ TODO
├── Exceptions/
│   ├── GatewayError.cs                 ✅ Created
│   ├── GatewayAuthenticationError.cs     ✅ Created
│   ├── GatewayInvalidRequestError.cs    ✅ Created
│   ├── GatewayRateLimitError.cs         ✅ Created
│   ├── GatewayModelNotFoundError.cs      ✅ Created
│   ├── GatewayInternalServerError.cs      ✅ Created
│   └── GatewayResponseError.cs         ✅ Created
├── Tools/
│   └── PerplexitySearchTool.cs        ⏳ TODO
├── GatewayMetadataFetcher.cs            ⏳ TODO
├── GatewayErrorFactory.cs              ⏳ TODO
├── GatewayAuthMethodParser.cs          ⏳ TODO
├── VercelEnvironment.cs                ⏳ TODO
└── Version.cs                          ⏳ TODO
```

---

## Implementation Notes

### TypeScript → C# Patterns

| TypeScript Pattern | C# Pattern |
|------------------|-------------|
| `z.object({...})` schema | C# records with validation attributes |
| `export type X = Y | Z` | `public record X(...)` or discriminated unions |
| `const symbol = Symbol.for(name)` | `private const string MarkerName = "...";` + field marker |
| `class X extends Error` | `class X : ApiCallError` |
| `static isInstance(error)` | `static bool IsInstance(object? error)` |
| `async function` | `async Task/Task<T>` |
| `Promise<T>` | `Task<T>` |
| `export function` | `public static` method |
| `namespace/module` | `namespace` or `static class` |
| `interface` | `interface` or `record` |
| `readonly property` | `{ get; }` property |
| `required` keyword | C# 11+ `required` or constructor checks |

### Authentication Flow (TypeScript)
```typescript
// 1. Try to get API key from options or env var
const apiKey = loadOptionalSetting({
  settingValue: options.apiKey,
  environmentVariableName: 'AI_GATEWAY_API_KEY',
});

// 2. If no API key, get OIDC token
const oidcToken = await getVercelOidcToken();

// 3. Return token and auth method
return { token: apiKey || oidcToken, authMethod: apiKey ? 'api-key' : 'oidc' };
```

### Authentication Flow (C# - proposed)
```csharp
// 1. Try to get API key from config or environment
var apiKey = config.ApiKey ?? Environment.GetEnvironmentVariable("AI_GATEWAY_API_KEY");

// 2. If no API key, get OIDC token (may not be available in .NET)
var oidcToken = await VercelEnvironment.GetOidcTokenAsync();

// 3. Return token and auth method
return new GatewayAuthToken {
    Token = apiKey ?? oidcToken,
    AuthMethod = apiKey != null ? "api-key" : "oidc"
};
```
