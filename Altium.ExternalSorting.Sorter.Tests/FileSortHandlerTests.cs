using System.Collections.Concurrent;
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
    private readonly FileSplitHandler _fileSplitHandler;
    private readonly ConcurrentBag<Task<string>> _sortTasks = [];

    public FileSortHandlerTests()
    {
        _fileSplitHandler = new FileSplitHandler(new SplitOptions { SplitFileSize = SplitFileSize, LineSeparator = "\n" });
        _fileSplitHandler.OnFileWritten += HandleFileWritten;
    }

    [Fact]
    public async Task SortFiles_WhenCalledWithUnsortedFiles_ShouldSortFiles()
    {
        Result _ = new FileBuilder().WithFilePath(TestFilePath).WithFileSize(FileSize).Build();
        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];

        try
        {
            splitFiles = await _fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = (await Task.WhenAll(_sortTasks)).AsReadOnly();

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
        IReadOnlyCollection<string> splitFiles = [];
        IReadOnlyCollection<string> sortedFiles = [];

        try
        {
            splitFiles = await _fileSplitHandler.SplitFileAsync(TestFilePath);
            sortedFiles = (await Task.WhenAll(_sortTasks)).AsReadOnly();

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

    
}