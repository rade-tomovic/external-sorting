using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;

namespace Altium.ExternalSorting.Runner;

public class ExternalSortingClient
{
    private readonly FileMergeHandler _fileMergeHandler;
    private readonly FileSortHandler _fileSortHandler;
    private readonly FileSplitHandler _fileSplitHandler;

    public ExternalSortingClient()
    {
        var splitOptions = new SplitOptions { SplitFileSize = 1024 * 1024 * 100, LineSeparator = "\n" };
        _fileSplitHandler = new FileSplitHandler(splitOptions);

        var sortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() };
        _fileSortHandler = new FileSortHandler(sortOptions);

        _fileMergeHandler = new FileMergeHandler();
    }

    public async Task<string> SortFileAsync(string inputFilePath, string outputFilePath)
    {
        IReadOnlyCollection<string> splitFiles = await _fileSplitHandler.SplitFileAsync(inputFilePath);
        IReadOnlyCollection<string> sortedFiles = await _fileSortHandler.SortFiles(splitFiles);
        string mergedFilePath = await _fileMergeHandler.MergeFilesAsync(sortedFiles, outputFilePath);

        return mergedFilePath;
    }
}