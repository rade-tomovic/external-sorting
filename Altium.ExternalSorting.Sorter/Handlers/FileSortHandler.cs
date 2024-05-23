using System.Collections.Concurrent;
using Altium.ExternalSorting.Sorter.Options;
using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSortHandler(SortOptions options)
{
    public async Task<IReadOnlyCollection<string>> SortFiles(IReadOnlyCollection<string> filePaths)
    {
        Log.Information("Starting to sort {count} files.", filePaths.Count);

        ConcurrentBag<string> sortedFilePaths = new();

        await Parallel.ForEachAsync(filePaths, async (filePath, cancellationToken) =>
        {
            string result = await SortFile(filePath, cancellationToken);
            sortedFilePaths.Add(result);
        });

        Log.Information("Finished sorting files. Sorted {count} files.", sortedFilePaths.Count);

        return sortedFilePaths.ToList().AsReadOnly();
    }

    public async Task<string> SortFile(string filePath, CancellationToken cancellationToken)
    {
        Log.Information("Starting to sort file: {filePath}", filePath);

        string[] lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        Log.Information("Read {count} lines from file: {filePath}", lines.Length, filePath);

        IEnumerable<string> sortedLines = options.LineSorter.SortLines(lines, options.Comparer);
        string sortedFilePath = Path.Combine(Path.GetDirectoryName(filePath),
            Path.GetFileNameWithoutExtension(filePath) + "_sorted" + Path.GetExtension(filePath));

        Log.Information("Writing sorted lines to file: {sortedFilePath}", sortedFilePath);

        await File.WriteAllLinesAsync(sortedFilePath, sortedLines, cancellationToken);
        Log.Information("Finished writing sorted lines to file: {sortedFilePath}", sortedFilePath);

        return sortedFilePath;
    }
}