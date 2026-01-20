#!/bin/bash

# Test script for MinimalApiExample endpoints
# Run this after starting the application with 'dotnet run'

BASE_URL="http://localhost:5000"

echo "=========================================="
echo "Testing AI SDK Minimal API"
echo "=========================================="
echo ""

echo "1. Health Check"
echo "----------------------------"
curl -s "$BASE_URL/health" | jq '.'
echo ""
echo ""

echo "2. API Info"
echo "----------------------------"
curl -s "$BASE_URL/api/info" | jq '.'
echo ""
echo ""

echo "3. Chat Completion (Non-Streaming)"
echo "----------------------------"
curl -s -X POST "$BASE_URL/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello, how are you?"}' | jq '.'
echo ""
echo ""

echo "4. Chat with System Message"
echo "----------------------------"
curl -s -X POST "$BASE_URL/api/chat" \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Explain minimal APIs",
    "systemMessage": "You are a helpful programming tutor",
    "maxTokens": 500,
    "temperature": 0.7
  }' | jq '.'
echo ""
echo ""

echo "5. Streaming Chat (Server-Sent Events)"
echo "----------------------------"
echo "Note: Streaming output will appear in real-time"
curl -N -X POST "$BASE_URL/api/chat/stream" \
  -H "Content-Type: application/json" \
  -d '{"message": "Tell me about AI SDK"}'
echo ""
echo ""

echo "=========================================="
echo "All tests completed!"
echo "=========================================="
