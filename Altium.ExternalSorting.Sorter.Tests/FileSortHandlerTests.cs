using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

namespace Altium.ExternalSorting.Sorter.Tests;

public class FileSortHandlerTests
{
    private const string TestFilePath = "sort_test.txt";
    private const long FileSize = 1024 * 1024 * 10;
    private const int SplitFileSize = 1024 * 1024 * 1;

    [Fact]
    public async Task SortFiles_WhenCalledWithUnsortedFiles_ShouldSortFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();
        var splitOptions = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(splitOptions);

        var sortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() };
        var fileSortHandler = new FileSortHandler(sortOptions);
        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];

        try
        {
            splitFiles = await fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = await fileSortHandler.SortFiles(splitFiles);

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
            CleanupFiles(splitFiles, sortedFiles);
        }
    }

    [Fact]
    public async Task SortFiles_WhenCalledWithEmptyFiles_ShouldReturnEmptyFiles()
    {
        await File.WriteAllTextAsync(TestFilePath, string.Empty);
        var splitOptions = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(splitOptions);
        var sortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() };
        var fileSortHandler = new FileSortHandler(sortOptions);
        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];

        try
        {
            splitFiles = await fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = await fileSortHandler.SortFiles(splitFiles);

            sortedFiles.Should().HaveCount(splitFiles.Count);

            foreach (string sortedFile in sortedFiles)
            {
                string[] sortedLines = await File.ReadAllLinesAsync(sortedFile);
                sortedLines.Should().BeEmpty();
            }
        }
        finally
        {
            CleanupFiles(splitFiles, sortedFiles);
        }
    }

    private static void CleanupFiles(IReadOnlyCollection<string> splitFiles, IReadOnlyCollection<string> sortedFiles)
    {
        File.Delete(TestFilePath);

        foreach (string filePath in splitFiles)
            File.Delete(filePath);

        foreach (string filePath in sortedFiles)
            File.Delete(filePath);
    }
}