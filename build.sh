#!/bin/bash

echo "======================================"
echo "AI SDK for .NET - Build Script"
echo "======================================"
echo ""

cd /home/ubuntu/work/ai-sdk/ai-sdk.net

echo "Step 1: Restore packages..."
dotnet restore
echo ""

echo "Step 2: Build AiSdk.Abstractions..."
dotnet build src/AiSdk.Abstractions/AiSdk.Abstractions.csproj -c Release
echo ""

echo "Step 3: Build AiSdk.Core..."
dotnet build src/AiSdk.Core/AiSdk.Core.csproj -c Release
echo ""

echo "Step 4: Build AiSdk..."
dotnet build src/AiSdk/AiSdk.csproj -c Release
echo ""

echo "Step 5: Build Tests..."
dotnet build tests/AiSdk.Abstractions.Tests/AiSdk.Abstractions.Tests.csproj -c Release
echo ""

echo "Step 6: Run Tests..."
dotnet test tests/AiSdk.Abstractions.Tests/AiSdk.Abstractions.Tests.csproj
echo ""

echo "======================================"
echo "Build Complete!"
echo "======================================"
echo ""
echo "Project Statistics:"
find src -name "*.cs" | wc -l | xargs echo "  Source files:"
find src -name "*.cs" -exec wc -l {} + | tail -1 | awk '{print "  Lines of code: " $1}'
echo ""
echo "Next steps:"
echo "  1. Implement provider packages (OpenAI, Anthropic, etc.)"
echo "  2. Add more tests"
echo "  3. Create real examples"
echo ""
