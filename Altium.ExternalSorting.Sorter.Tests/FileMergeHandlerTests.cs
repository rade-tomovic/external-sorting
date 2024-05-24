using System.Collections.Concurrent;
using Altium.ExternalSorting.FileGenerator;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using FluentAssertions;

namespace Altium.ExternalSorting.Sorter.Tests;

public class FileMergeHandlerTests
{
    private const string TestFilePath = "merge_test.txt";
    private const long FileSize = 1024 * 1024 * 10;
    private const int SplitFileSize = 1024 * 1024 * 1;
    private readonly FileSplitHandler _fileSplitHandler;
    private readonly ConcurrentBag<Task<string>> _sortTasks = [];

    public FileMergeHandlerTests()
    {
        _fileSplitHandler = new FileSplitHandler(new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" });
        _fileSplitHandler.OnFileWritten += HandleFileWritten;
    }

    [Fact]
    public async Task MergeFiles_WhenCalledWithSortedFiles_ShouldMergeFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();
        string? mergedFilePath = "merged.txt";
        var fileMergeHandler = new FileMergeHandler(new MergeOptions { BufferSize = 1024, OutputFile = mergedFilePath });

        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];
        

        try
        {
            splitFiles = await _fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = (await Task.WhenAll(_sortTasks)).AsReadOnly();
            mergedFilePath = await fileMergeHandler.MergeFilesAsync(sortedFiles.ToList());

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

            if (File.Exists(mergedFilePath))
                File.Delete(mergedFilePath);
        }
    }

    public void Dispose()
    {
        _fileSplitHandler.OnFileWritten -= HandleFileWritten;
    }

    private void HandleFileWritten(string filepath)
    {
        var fileSortHandler = new FileSortHandler(new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() });
        Task<string> sortTask = fileSortHandler.SortFile(filepath, CancellationToken.None);
        _sortTasks.Add(sortTask);
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