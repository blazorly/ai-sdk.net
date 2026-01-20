using AiSdk.Core.Utilities;
using FluentAssertions;
using Xunit;

namespace AiSdk.Core.Tests;

public class IdGeneratorTests
{
    [Fact]
    public void Generate_WithDefaults_ShouldReturnIdWithDefaultPrefix()
    {
        var id = IdGenerator.Generate();
        id.Should().StartWith("aitxt-");
    }

    [Fact]
    public void Generate_WithDefaults_ShouldHaveCorrectLength()
    {
        var id = IdGenerator.Generate();
        id.Should().HaveLength(30); // "aitxt-" (6) + 24 chars
    }

    [Theory]
    [InlineData("msg")]
    [InlineData("req")]
    [InlineData("custom")]
    public void Generate_WithCustomPrefix_ShouldUsePrefix(string prefix)
    {
        var id = IdGenerator.Generate(prefix);
        id.Should().StartWith($"{prefix}-");
    }

    [Fact]
    public void Generate_WithNullPrefix_ShouldThrowArgumentNullException()
    {
        var act = () => IdGenerator.Generate(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("prefix");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public void Generate_WithCustomSize_ShouldHaveCorrectLength(int size)
    {
        var id = IdGenerator.Generate(size: size);
        id.Should().HaveLength(6 + size);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Generate_WithInvalidSize_ShouldThrowArgumentOutOfRangeException(int size)
    {
        var act = () => IdGenerator.Generate(size: size);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("size");
    }

    [Fact]
    public void Generate_MultipleCalls_ShouldProduceDifferentIds()
    {
        var id1 = IdGenerator.Generate();
        var id2 = IdGenerator.Generate();
        var id3 = IdGenerator.Generate();

        id1.Should().NotBe(id2);
        id2.Should().NotBe(id3);
        id1.Should().NotBe(id3);
    }

    [Fact]
    public void Generate_ManyIds_ShouldAllBeUnique()
    {
        const int count = 1000;
        var ids = Enumerable.Range(0, count)
            .Select(_ => IdGenerator.Generate())
            .ToList();

        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Generate_ShouldContainOnlyValidCharacters()
    {
        var ids = Enumerable.Range(0, 100).Select(_ => IdGenerator.Generate()).ToList();

        ids.Should().AllSatisfy(id =>
        {
            var randomPart = id.Substring(6);
            randomPart.Should().MatchRegex(@"^[0-9A-Za-z]+$");
        });
    }

    [Fact]
    public void GenerateSimple_WithDefaults_ShouldReturnIdWithoutPrefix()
    {
        var id = IdGenerator.GenerateSimple();
        id.Should().NotContain("-");
        id.Should().HaveLength(24);
    }

    [Fact]
    public void GenerateSimple_ShouldContainOnlyValidCharacters()
    {
        var id = IdGenerator.GenerateSimple();
        id.Should().MatchRegex(@"^[0-9A-Za-z]+$");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public void GenerateSimple_WithCustomSize_ShouldHaveCorrectLength(int size)
    {
        var id = IdGenerator.GenerateSimple(size);
        id.Should().HaveLength(size);
    }

    [Fact]
    public void GenerateSimple_MultipleCalls_ShouldProduceUniqueIds()
    {
        var ids = Enumerable.Range(0, 1000)
            .Select(_ => IdGenerator.GenerateSimple())
            .ToList();

        ids.Should().OnlyHaveUniqueItems();
    }
}
