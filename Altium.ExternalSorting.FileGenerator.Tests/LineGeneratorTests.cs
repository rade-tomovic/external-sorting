using FluentAssertions;

namespace Altium.ExternalSorting.FileGenerator.Tests;

public class LineGeneratorTests
{
    [Fact]
    public void Build_WithoutContentGenerator_ReturnsExpectedFormat()
    {
        var lineGenerator = new LineGenerator();
        string result = lineGenerator.Build();

        result.Should().MatchRegex(@"^\d+\. .+$");
    }

    [Fact]
    public void Build_WithContentGenerator_ReturnsExpectedFormat()
    {
        var lineGenerator = new LineGenerator(() => "test content");
        string result = lineGenerator.Build();

        result.Should().MatchRegex(@"^\d+\. test content$");
    }
}