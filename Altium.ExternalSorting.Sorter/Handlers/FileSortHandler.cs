using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSortHandler
{
    private readonly LineSorter _lineSorter = new();

    public async Task<IReadOnlyCollection<string>> SortFiles(IReadOnlyCollection<string> filePaths)
    {
        Log.Information("Starting to sort files.");

        List<string> sortedFilePaths = new(filePaths.Count);

        foreach (string filePath in filePaths)
        {
            Log.Information($"Reading lines from file: {filePath}");

            string[] lines = await File.ReadAllLinesAsync(filePath);
            string[] sortedLines = _lineSorter.SortLines(lines);
            string sortedFilePath = Path.Combine(Path.GetDirectoryName(filePath),
                Path.GetFileNameWithoutExtension(filePath) + "_sorted" + Path.GetExtension(filePath));

            Log.Information($"Writing sorted lines to file: {sortedFilePath}");

            await File.WriteAllLinesAsync(sortedFilePath, sortedLines);
            sortedFilePaths.Add(sortedFilePath);
            File.Delete(filePath);
        }

        Log.Information("Finished sorting files.");
        
        return sortedFilePaths.AsReadOnly();
    }
}