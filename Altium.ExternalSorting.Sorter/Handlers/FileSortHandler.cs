using Altium.ExternalSorting.Sorter.Options;
using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSortHandler
{
    private readonly SortOptions _options;

    public FileSortHandler(SortOptions options)
    {
        _options = options;
        Log.Information("{fileSortHandler} initialized with options: {@options}", nameof(FileSortHandler), options);
    }

    public async Task<string> SortFile(string filePath, CancellationToken cancellationToken)
    {
        Log.Information("Starting to sort file: {filePath}", filePath);

        string[] lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        Log.Information("Read {count} lines from file: {filePath}", lines.Length, filePath);

        IEnumerable<string> sortedLines = _options.LineSorter.SortLines(lines, _options.Comparer);
        string sortedFilePath = Path.Combine(Path.GetDirectoryName(filePath),
            Path.GetFileNameWithoutExtension(filePath) + "_sorted" + Path.GetExtension(filePath));

        Log.Information("Writing sorted lines to file: {sortedFilePath}", sortedFilePath);

        await File.WriteAllLinesAsync(sortedFilePath, sortedLines, cancellationToken);
        Log.Information("Finished writing sorted lines to file: {sortedFilePath}", sortedFilePath);

        if(File.Exists(filePath))
            File.Delete(filePath);

        return sortedFilePath;
    }
}