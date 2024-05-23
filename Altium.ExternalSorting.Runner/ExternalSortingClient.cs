using System.Collections.Concurrent;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;

namespace Altium.ExternalSorting.Runner;

public class ExternalSortingClient
{
    private readonly FileMergeHandler _fileMergeHandler;
    private readonly FileSortHandler _fileSortHandler;
    private readonly FileSplitHandler _fileSplitHandler;
    private readonly ConcurrentBag<Task<string>> _sortTasks = [];

    public ExternalSortingClient()
    {
        var splitOptions = new SplitOptions { SplitFileSize = 1024 * 1024 * 100, LineSeparator = "\n" };
        _fileSplitHandler = new FileSplitHandler(splitOptions);
        _fileSplitHandler.OnFileWritten += HandleFileWritten;

        var sortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() };
        _fileSortHandler = new FileSortHandler(sortOptions);

        _fileMergeHandler = new FileMergeHandler();
    }

    private void HandleFileWritten(string filepath)
    {
        Task<string> sortTask = _fileSortHandler.SortFile(filepath, CancellationToken.None);

        _sortTasks.Add(sortTask);
    }

    public async Task SortFileAsync(string inputFilePath, string outputFilePath)
    {
        await _fileSplitHandler.SplitFileAsync(inputFilePath);
        string[] results = await Task.WhenAll(_sortTasks);
        string mergedFilePath = await _fileMergeHandler.MergeFilesAsync(results, outputFilePath);
    }
}