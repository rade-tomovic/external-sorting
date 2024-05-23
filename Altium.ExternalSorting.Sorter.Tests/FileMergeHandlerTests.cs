using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

namespace Altium.ExternalSorting.Sorter.Tests;

public class FileMergeHandlerTests
{
    private const string TestFilePath = "merge_test.txt";
    private const long FileSize = 1024 * 1024 * 100;
    private const int SplitFileSize = 1024 * 1024 * 10;

    [Fact]
    public async Task MergeFiles_WhenCalledWithSortedFiles_ShouldMergeFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();
        var splitOptions = new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" };
        var fileSplitHandler = new FileSplitHandler(splitOptions);

        var sortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() };
        var fileSortHandler = new FileSortHandler(sortOptions);
        var fileMergeHandler = new FileMergeHandler();

        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];
        string mergedFilePath = "";

        try
        {
            splitFiles = await fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = await fileSortHandler.SortFiles(splitFiles);
            mergedFilePath = await fileMergeHandler.MergeFilesAsync(sortedFiles, "merged.txt");

            string[] mergedLines = await File.ReadAllLinesAsync(mergedFilePath);

            for (int i = 0; i < mergedLines.Length - 1; i++)
            {
                var comparer = new LineComparer();
                int comparisonResult = comparer.Compare(mergedLines[i], mergedLines[i + 1]);
                comparisonResult.Should().BeLessOrEqualTo(0);
            }
        }
        finally
        {
            CleanupFiles(splitFiles, sortedFiles);

            //if (File.Exists(mergedFilePath))
            //    File.Delete(mergedFilePath);
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