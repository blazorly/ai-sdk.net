using AiSdk.Abstractions;
using FluentAssertions;
using Xunit;

namespace AiSdk.Abstractions.Tests;

public class ErrorTests
{
    [Fact]
    public void ApiCallError_Should_Have_Correct_ErrorName()
    {
        // Arrange & Act
        var error = new ApiCallError("Test error");

        // Assert
        error.ErrorName.Should().Be("AI_ApiCallError");
        error.Message.Should().Be("Test error");
    }

    [Fact]
    public void ApiCallError_IsInstance_Should_Return_True_For_ApiCallError()
    {
        // Arrange
        var error = new ApiCallError("Test error");

        // Act
        var isInstance = ApiCallError.IsInstance(error);

        // Assert
        isInstance.Should().BeTrue();
    }

    [Fact]
    public void InvalidPromptError_Should_Have_Correct_ErrorName()
    {
        // Arrange & Act
        var error = new InvalidPromptError("Invalid prompt");

        // Assert
        error.ErrorName.Should().Be("AI_InvalidPromptError");
        error.Message.Should().Be("Invalid prompt");
    }

    [Fact]
    public void NoSuchToolError_Should_Include_ToolName()
    {
        // Arrange & Act
        var error = new NoSuchToolError("Tool not found")
        {
            ToolName = "my_tool"
        };

        // Assert
        error.ErrorName.Should().Be("AI_NoSuchToolError");
        error.ToolName.Should().Be("my_tool");
    }
}
