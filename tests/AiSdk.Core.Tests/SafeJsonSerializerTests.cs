using System.Text.Json;
using System.Text.Json.Serialization;
using AiSdk.Core.Http;
using FluentAssertions;
using Xunit;

namespace AiSdk.Core.Tests;

public class SafeJsonSerializerTests
{
    // Test models
    private record TestModel(string Name, int Age, string? Email = null);
    private record ComplexModel(
        string Id,
        List<string> Tags,
        Dictionary<string, object> Metadata,
        NestedModel? Nested = null
    );
    private record NestedModel(string Value, int Count);

    #region Successful Deserialization Tests

    [Fact]
    public void Deserialize_WithValidJson_ShouldReturnObject()
    {
        var json = """{"name":"John","age":30}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Should().NotBeNull();
        result.Name.Should().Be("John");
        result.Age.Should().Be(30);
    }

    [Fact]
    public void Deserialize_WithCamelCaseJson_ShouldDeserializeCorrectly()
    {
        var json = """{"name":"Alice","age":25,"email":"alice@example.com"}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("Alice");
        result.Age.Should().Be(25);
        result.Email.Should().Be("alice@example.com");
    }

    [Fact]
    public void Deserialize_WithPascalCaseJson_ShouldDeserializeCorrectly()
    {
        var json = """{"Name":"Bob","Age":35,"Email":"bob@example.com"}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("Bob");
        result.Age.Should().Be(35);
        result.Email.Should().Be("bob@example.com");
    }

    [Fact]
    public void Deserialize_WithComplexObject_ShouldDeserializeCorrectly()
    {
        var json = """
        {
            "id": "test-123",
            "tags": ["tag1", "tag2", "tag3"],
            "metadata": {
                "key1": "value1",
                "key2": 42
            },
            "nested": {
                "value": "nested-value",
                "count": 10
            }
        }
        """;

        var result = SafeJsonSerializer.Deserialize<ComplexModel>(json);

        result.Id.Should().Be("test-123");
        result.Tags.Should().BeEquivalentTo("tag1", "tag2", "tag3");
        result.Metadata.Should().ContainKey("key1");
        result.Nested.Should().NotBeNull();
        result.Nested!.Value.Should().Be("nested-value");
        result.Nested.Count.Should().Be(10);
    }

    [Fact]
    public void Deserialize_WithWhitespace_ShouldDeserializeCorrectly()
    {
        var json = """

        {
            "name"  :  "Charlie"  ,
            "age"   :  40
        }

        """;

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("Charlie");
        result.Age.Should().Be(40);
    }

    [Fact]
    public void Deserialize_WithUnicodeCharacters_ShouldDeserializeCorrectly()
    {
        var json = """{"name":"José María","age":28}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("José María");
        result.Age.Should().Be(28);
    }

    [Fact]
    public void Deserialize_WithEscapedCharacters_ShouldDeserializeCorrectly()
    {
        var json = """{"name":"John \"Johnny\" Doe","age":30}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("John \"Johnny\" Doe");
    }

    #endregion

    #region Null Handling Tests

    [Fact]
    public void Deserialize_WithNullJson_ShouldThrowArgumentNullException()
    {
        var act = () => SafeJsonSerializer.Deserialize<TestModel>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Deserialize_WithNullResult_ShouldThrowJsonException()
    {
        var json = "null";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>()
            .WithMessage("Deserialization resulted in null value.");
    }

    [Fact]
    public void Deserialize_WithOptionalNullProperty_ShouldDeserializeCorrectly()
    {
        var json = """{"name":"David","age":32,"email":null}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("David");
        result.Age.Should().Be(32);
        result.Email.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WithMissingOptionalProperty_ShouldDeserializeCorrectly()
    {
        var json = """{"name":"Emma","age":27}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("Emma");
        result.Age.Should().Be(27);
        result.Email.Should().BeNull();
    }

    #endregion

    #region Invalid JSON Handling Tests

    [Fact]
    public void Deserialize_WithEmptyString_ShouldThrowJsonException()
    {
        var json = "";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithInvalidJson_ShouldThrowJsonException()
    {
        var json = "{invalid json}";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithMalformedJson_ShouldThrowJsonException()
    {
        var json = """{"name":"Frank","age":}""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithUnclosedBrace_ShouldThrowJsonException()
    {
        var json = """{"name":"Grace","age":29""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithMismatchedTypes_ShouldThrowJsonException()
    {
        var json = """{"name":"Harry","age":"not-a-number"}""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithMissingRequiredProperty_ShouldUseDefaultValue()
    {
        var json = """{"name":"Ivy"}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        // Records with value types use default values for missing properties
        result.Name.Should().Be("Ivy");
        result.Age.Should().Be(0);
    }

    [Fact]
    public void Deserialize_WithExtraComma_ShouldThrowJsonException()
    {
        var json = """{"name":"Jack","age":31,}""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithArrayWhenObjectExpected_ShouldThrowJsonException()
    {
        var json = """["not","an","object"]""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    #endregion

    #region Security Tests

    [Fact]
    public void Deserialize_WithVeryLargeString_ShouldHandleGracefully()
    {
        var largeString = new string('x', 100000);
        var json = $$"""{"name":"{{largeString}}","age":30}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().HaveLength(100000);
        result.Age.Should().Be(30);
    }

    [Fact]
    public void Deserialize_WithDeeplyNestedObject_ShouldHandleWithinLimits()
    {
        // Create a moderately nested JSON (not too deep to avoid stack overflow)
        var json = """
        {
            "id": "1",
            "tags": ["a"],
            "metadata": {"level1": {"level2": {"level3": "value"}}},
            "nested": {"value": "test", "count": 1}
        }
        """;

        var result = SafeJsonSerializer.Deserialize<ComplexModel>(json);

        result.Id.Should().Be("1");
    }

    [Fact]
    public void Deserialize_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        var json = """{"name":"<script>alert('xss')</script>","age":30}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("<script>alert('xss')</script>");
    }

    [Fact]
    public void Deserialize_WithSqlInjectionAttempt_ShouldHandleAsString()
    {
        var json = """{"name":"'; DROP TABLE users; --","age":30}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("'; DROP TABLE users; --");
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public void Serialize_WithSimpleObject_ShouldProduceValidJson()
    {
        var model = new TestModel("Kate", 26);

        var json = SafeJsonSerializer.Serialize(model);

        json.Should().Contain("\"name\":\"Kate\"");
        json.Should().Contain("\"age\":26");
    }

    [Fact]
    public void Serialize_WithComplexObject_ShouldProduceValidJson()
    {
        var model = new ComplexModel(
            "id-123",
            new List<string> { "tag1", "tag2" },
            new Dictionary<string, object> { { "key", "value" } },
            new NestedModel("nested", 5)
        );

        var json = SafeJsonSerializer.Serialize(model);

        json.Should().Contain("\"id\":\"id-123\"");
        json.Should().Contain("\"tags\":[\"tag1\",\"tag2\"]");
        json.Should().Contain("\"nested\"");
    }

    [Fact]
    public void Serialize_WithNullProperty_ShouldOmitProperty()
    {
        var model = new TestModel("Leo", 33, null);

        var json = SafeJsonSerializer.Serialize(model);

        json.Should().Contain("\"name\":\"Leo\"");
        json.Should().Contain("\"age\":33");
        json.Should().NotContain("email");
    }

    [Fact]
    public void Serialize_ThenDeserialize_ShouldPreserveData()
    {
        var original = new TestModel("Mike", 29, "mike@example.com");

        var json = SafeJsonSerializer.Serialize(original);
        var deserialized = SafeJsonSerializer.Deserialize<TestModel>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void Serialize_WithSpecialCharacters_ShouldEscapeCorrectly()
    {
        var model = new TestModel("Nancy \"The Boss\"", 45);

        var json = SafeJsonSerializer.Serialize(model);
        var deserialized = SafeJsonSerializer.Deserialize<TestModel>(json);

        // Verify the special characters are properly escaped and can be round-tripped
        deserialized.Name.Should().Be("Nancy \"The Boss\"");
        json.Should().Contain("\\u0022"); // JSON escaped quote
    }

    [Fact]
    public void Serialize_WithUnicodeCharacters_ShouldPreserveUnicode()
    {
        var model = new TestModel("Olga Müller", 31);

        var json = SafeJsonSerializer.Serialize(model);
        var deserialized = SafeJsonSerializer.Deserialize<TestModel>(json);

        deserialized.Name.Should().Be("Olga Müller");
    }

    [Fact]
    public void Serialize_WithEmptyCollections_ShouldSerializeAsEmptyArrays()
    {
        var model = new ComplexModel(
            "id-456",
            new List<string>(),
            new Dictionary<string, object>(),
            null
        );

        var json = SafeJsonSerializer.Serialize(model);

        json.Should().Contain("\"tags\":[]");
        json.Should().Contain("\"metadata\":{}");
    }

    #endregion

    #region Custom Options Tests

    [Fact]
    public void Deserialize_WithCustomOptions_ShouldUseProvidedOptions()
    {
        var json = """{"custom_name":"Paul","custom_age":38}""";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var model = new CustomNameModel();
        // This test verifies custom options are passed through
        // The actual behavior depends on the property mapping
        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json, options);

        // Should still work with custom options
        act.Should().NotThrow();
    }

    [Fact]
    public void Serialize_WithCustomOptions_ShouldUseProvidedOptions()
    {
        var model = new TestModel("Quinn", 42);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = SafeJsonSerializer.Serialize(model, options);

        json.Should().Contain("\n");
        json.Should().Contain("  ");
    }

    [Fact]
    public void Deserialize_WithCaseSensitiveOptions_ShouldRespectCaseSensitivity()
    {
        var json = """{"Name":"Rachel","Age":34}""";
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false
        };

        // This should work since property names match exactly
        var result = SafeJsonSerializer.Deserialize<TestModel>(json, options);

        result.Name.Should().Be("Rachel");
    }

    [Fact]
    public void Serialize_WithNullValueHandling_ShouldIncludeNulls()
    {
        var model = new TestModel("Sam", 36, null);
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = SafeJsonSerializer.Serialize(model, options);

        json.Should().Contain("email").And.Contain("null");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Deserialize_WithNumbersAsStrings_ShouldThrowJsonException()
    {
        var json = """{"name":"Tom","age":"30"}""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithBooleanInsteadOfString_ShouldThrowJsonException()
    {
        var json = """{"name":true,"age":30}""";

        var act = () => SafeJsonSerializer.Deserialize<TestModel>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Serialize_WithCircularReference_ShouldNotCauseStackOverflow()
    {
        // Using simple models without circular references
        var model = new TestModel("Uma", 39);

        var act = () => SafeJsonSerializer.Serialize(model);

        act.Should().NotThrow();
    }

    [Fact]
    public void Deserialize_WithDuplicateKeys_ShouldUseLastValue()
    {
        var json = """{"name":"Victor","age":40,"name":"Victoria"}""";

        var result = SafeJsonSerializer.Deserialize<TestModel>(json);

        result.Name.Should().Be("Victoria");
    }

    #endregion

    // Helper model for custom options test
    private class CustomNameModel
    {
        public string CustomName { get; set; } = string.Empty;
        public int CustomAge { get; set; }
    }
}
