using System.Text;
using Altium.ExternalSorting.Sorter.Options;
using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSplitHandler
{
    public delegate void FileWrittenHandler(string filePath);

    private readonly SplitOptions _options;

    public FileSplitHandler(SplitOptions options)
    {
        _options = options;
        Log.Information("{fileSplitHandler} initialized with options: {@options}", nameof(FileSplitHandler), options);
    }

    public event FileWrittenHandler? OnFileWritten;

    public async Task<IReadOnlyCollection<string>> SplitFileAsync(string sourceFilePath)
    {
        Log.Information("Starting to split file: {sourceFilePath}", sourceFilePath);
        List<string> splitFilePaths = [];

        await using var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(sourceStream);
        int fileIndex = 0;
        long currentFileSize = 0L;
        StreamWriter? writer = null;

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            if (line == null)
                continue;

            int lineSize = Encoding.UTF8.GetByteCount(line) + _options.LineSeparator.Length;

            if (currentFileSize + lineSize > _options.SplitFileSize || writer == null)
            {
                if (writer != null)
                {
                    await writer.DisposeAsync();
                    Log.Information("Finished writing to file: {filePath}", splitFilePaths[^1]);

                    if (OnFileWritten != null)
                        OnFileWritten.Invoke(splitFilePaths[^1]);
                }

                string splitFilePath = GetSplitFilePath(sourceFilePath, fileIndex++);
                splitFilePaths.Add(splitFilePath);
                var splitFileStream = new FileStream(splitFilePath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(splitFileStream);
                currentFileSize = 0L;
                Log.Information("Started writing to new file: {filePath}", splitFilePath);
            }

            await writer.WriteLineAsync(line);
            currentFileSize += lineSize;
        }

        if (writer != null)
        {
            await writer.DisposeAsync();
            Log.Information("Finished writing to file: {filePath}", splitFilePaths[^1]);

            if (OnFileWritten != null)
                OnFileWritten.Invoke(splitFilePaths[^1]);
        }

        Log.Information("Finished splitting file: {sourceFilePath}. Created {count} files.", sourceFilePath, splitFilePaths.Count);
        return splitFilePaths.AsReadOnly();
    }

    private string GetSplitFilePath(string sourceFilePath, int fileIndex)
    {
        string? directory = Path.GetDirectoryName(sourceFilePath);
        string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
        string extension = Path.GetExtension(sourceFilePath);
        return Path.Combine(directory ?? string.Empty, $"{fileName}_{fileIndex}{extension}");
    }
}