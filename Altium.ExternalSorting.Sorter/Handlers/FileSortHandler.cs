using Altium.ExternalSorting.Sorter.Options;
using Serilog;
using System.Collections.Concurrent;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSortHandler(SortOptions options)
{
    public async Task<IReadOnlyCollection<string>> SortFiles(IReadOnlyCollection<string> filePaths)
    {
        Log.Information("Starting to sort files.");

        ConcurrentBag<string> sortedFilePaths = new();

        await Parallel.ForEachAsync(filePaths, async (filePath, cancellationToken) =>
        {
            Log.Information($"Reading lines from file: {filePath}");

            string[] lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            IEnumerable<string> sortedLines = options.LineSorter.SortLines(lines, options.Comparer);
            string sortedFilePath = Path.Combine(Path.GetDirectoryName(filePath),
                Path.GetFileNameWithoutExtension(filePath) + "_sorted" + Path.GetExtension(filePath));

            Log.Information($"Writing sorted lines to file: {sortedFilePath}");

            await File.WriteAllLinesAsync(sortedFilePath, sortedLines, cancellationToken);
            sortedFilePaths.Add(sortedFilePath);
            File.Delete(filePath);
        });

        Log.Information("Finished sorting files.");
        
        return sortedFilePaths.ToList().AsReadOnly();
    }
}