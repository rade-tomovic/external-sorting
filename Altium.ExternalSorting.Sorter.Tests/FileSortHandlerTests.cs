using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

namespace Altium.ExternalSorting.Sorter.Tests;

public class FileSortHandlerTests
{
    private const string TestFilePath = "test.txt";
    private const long FileSize = 1024 * 1024 * 100;
    private const int SplitFileSize = 1024 * 1024 * 10;

    [Fact]
    public async Task SortFiles_WhenCalledWithUnsortedFiles_ShouldSortFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();
        var splitOptions = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(splitOptions);
        var fileSortHandler = new FileSortHandler();
        List<string> splitFiles = [];

        try
        {
            splitFiles = await fileSplitHandler.SplitFileAsync(TestFilePath);
            IReadOnlyCollection<string> sortedFiles = await fileSortHandler.SortFiles(splitFiles);

            sortedFiles.Should().HaveCount(splitFiles.Count);

            foreach (string sortedFile in sortedFiles)
            {
                string[] sortedLines = await File.ReadAllLinesAsync(sortedFile);
                for (int i = 0; i < sortedLines.Length - 1; i++)
                {
                    var comparer = new LineComparer();
                    int comparisonResult = comparer.Compare(sortedLines[i], sortedLines[i + 1]);
                    comparisonResult.Should().BeLessOrEqualTo(0);
                }
            }
        }
        finally
        {
            File.Delete(TestFilePath);
            foreach (string filePath in splitFiles)
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task SortFiles_WhenCalledWithEmptyFiles_ShouldReturnEmptyFiles()
    {
        await File.WriteAllTextAsync(TestFilePath, string.Empty);
        var splitOptions = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(splitOptions);
        var fileSortHandler = new FileSortHandler();
        List<string> splitFiles = [];

        try
        {
            splitFiles = await fileSplitHandler.SplitFileAsync(TestFilePath);
            IReadOnlyCollection<string> sortedFiles = await fileSortHandler.SortFiles(splitFiles);

            sortedFiles.Should().HaveCount(splitFiles.Count);

            foreach (string sortedFile in sortedFiles)
            {
                string[] sortedLines = await File.ReadAllLinesAsync(sortedFile);
                sortedLines.Should().BeEmpty();
            }
        }
        finally
        {
            File.Delete(TestFilePath);
            foreach (string filePath in splitFiles)
                File.Delete(filePath);
        }
    }
}