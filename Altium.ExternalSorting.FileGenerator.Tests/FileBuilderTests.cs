using FluentAssertions;

namespace Altium.ExternalSorting.FileGenerator.Tests;

public class FileBuilderTests : IDisposable
{
    private readonly string _filePath = Path.GetTempFileName();

    public void Dispose()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void Build_WithValidParameters_CreatesFileWithExpectedSize()
    {
        FileBuilder fileBuilder = new FileBuilder().WithFilePath(_filePath).WithFileSize(1024);

        Result result = fileBuilder.Build();

        var fileInfo = new FileInfo(_filePath);
        fileInfo.Exists.Should().BeTrue();
        fileInfo.Length.Should().BeGreaterOrEqualTo(1024);
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("File created successfully.");
    }

    [Fact]
    public void Build_WithInvalidFilePath_ThrowsException()
    {
        Action act = () => new FileBuilder().WithFilePath(string.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Build_WithInvalidFileSize_ThrowsException()
    {
        Action act = () => new FileBuilder().WithFileSize(0);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Build_WithNonExistentFilePath_ReturnsFailureResult()
    {
        FileBuilder fileBuilder = new FileBuilder().WithFilePath("/non/existent/path").WithFileSize(1024);

        Result result = fileBuilder.Build();

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().StartWith("An error occurred:");
    }
}