using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

public class FileSplitHandlerTests
{
    private const string TestFilePath = "test.txt";
    private const long FileSize = 1024 * 1024 * 100;
    private const int SplitFileSize = 1024 * 1024 * 10;

    [Fact]
    public async Task SplitFile_WhenCalledWithLargeFile_ShouldSplitFileIntoSmallerFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();

        var options = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(options);

        List<string> result = [];

        try
        {
            result = await fileSplitHandler.SplitFileAsync(TestFilePath);
            result.Should().HaveCount((int)(FileSize / SplitFileSize));
        }
        finally
        {
            File.Delete(TestFilePath);
            foreach (string filePath in result)
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task SplitFile_WhenCalledWithEmptyFile_ShouldNotCreateAnyFiles()
    {
        // Arrange
        await File.WriteAllTextAsync(TestFilePath, string.Empty);

        var options = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(options);

        try
        {
            List<string> result = await fileSplitHandler.SplitFileAsync(TestFilePath);

            result.Should().BeEmpty();
        }
        finally
        {
            File.Delete(TestFilePath);
        }
    }
}