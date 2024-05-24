using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

public class FileSplitHandlerTests
{
    private const string TestFilePath = "split_test.txt";
    private const long FileSize = 1024 * 1024 * 10;
    private const int SplitFileSize = 1024 * 1024 * 1;

    [Fact]
    public async Task SplitFile_WhenCalledWithLargeFile_ShouldSplitFileIntoSmallerFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();

        var options = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(options);

        IReadOnlyCollection<string> result = [];

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
        await File.WriteAllTextAsync(TestFilePath, string.Empty);

        var options = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(options);

        try
        {
            IReadOnlyCollection<string> result = await fileSplitHandler.SplitFileAsync(TestFilePath);

            result.Should().BeEmpty();
        }
        finally
        {
            File.Delete(TestFilePath);
        }
    }
}