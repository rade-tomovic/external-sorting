using System.Collections.Concurrent;
using Altium.ExternalSorting.Sorter.Handlers;

namespace Altium.ExternalSorting.Runner;

public class ExternalSortingClient : IDisposable
{
    private readonly RunnerOptions _options;
    private readonly FileSortHandler _fileSortHandler;
    private readonly FileSplitHandler _fileSplitHandler;
    private readonly ConcurrentBag<Task<string>> _sortTasks = [];

    public ExternalSortingClient(RunnerOptions options)
    {
        _options = options;
        _fileSplitHandler = new FileSplitHandler(options.SplitOptions);
        _fileSplitHandler.OnFileWritten += HandleFileWritten;
        _fileSortHandler = new FileSortHandler(options.SortOptions);
    }

    public async Task<string?> SortFileAsync()
    {
        await _fileSplitHandler.SplitFileAsync(_options.InputFilePath);
        string[] sortedFilePaths = await Task.WhenAll(_sortTasks);
        var fileMergeHandler = new FileMergeHandler(_options.MergeOptions);

        string? result = await fileMergeHandler.MergeFilesAsync(sortedFilePaths.ToList());

        foreach (string filePath in sortedFilePaths)
            File.Delete(filePath);

        return result;
    }

    public void Dispose()
    {
        _fileSplitHandler.OnFileWritten -= HandleFileWritten;
    }

    private void HandleFileWritten(string filepath)
    {
        Task<string> sortTask = _fileSortHandler.SortFile(filepath, CancellationToken.None);

        _sortTasks.Add(sortTask);
    }
}