using System.Text;
using Altium.ExternalSorting.Sorter.Options;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileSplitHandler
{
    private readonly SplitOptions _options;

    public FileSplitHandler(SplitOptions options)
    {
        _options = options;
    }

    public async Task<List<string>> SplitFileAsync(string sourceFilePath)
    {
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
                }
                string splitFilePath = GetSplitFilePath(sourceFilePath, fileIndex++);
                splitFilePaths.Add(splitFilePath);
                var splitFileStream = new FileStream(splitFilePath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(splitFileStream);
                currentFileSize = 0L;
            }

            await writer.WriteLineAsync(line);
            currentFileSize += lineSize;
        }

        if (writer != null)
        {
            await writer.DisposeAsync();
        }

        return splitFilePaths;
    }

    private string GetSplitFilePath(string sourceFilePath, int fileIndex)
    {
        string? directory = Path.GetDirectoryName(sourceFilePath);
        string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
        string extension = Path.GetExtension(sourceFilePath);
        return Path.Combine(directory ?? string.Empty, $"{fileName}_{fileIndex}{extension}");
    }
}