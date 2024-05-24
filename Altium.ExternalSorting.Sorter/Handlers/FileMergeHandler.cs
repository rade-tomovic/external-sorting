using Altium.ExternalSorting.Sorter.Options;
using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class FileMergeHandler
{
    private readonly MergeOptions _options;

    public FileMergeHandler(MergeOptions options)
    {
        _options = options;
        Log.Information("{fileMergeHandler} initialized with options: {@options}", nameof(FileMergeHandler), options);
    }

    public async Task<string?> MergeFilesAsync(List<string> filePaths)
    {
        PriorityQueue<(string Line, StreamReader Stream), string> queue = new(new LineComparer());
        Dictionary<StreamReader, Queue<string>> buffers = new();

        Log.Information("Starting to merge {count} files into {outputFilePath}", filePaths.Count, _options.OutputFile);

        try
        {
            foreach (string filePath in filePaths)
            {
                var stream = new StreamReader(filePath);
                Queue<string> buffer = new();
                buffers[stream] = buffer;

                await FillBufferAsync(stream, buffer);

                if (buffer.Any())
                {
                    string line = buffer.Dequeue();
                    queue.Enqueue((line, stream), line);
                }
            }

            await using var outputStream = new StreamWriter(_options.OutputFile);

            while (queue.Count > 0)
            {
                (string line, StreamReader stream) = queue.Dequeue();
                await outputStream.WriteLineAsync(line);

                Queue<string> buffer = buffers[stream];
                if (!buffer.Any()) await FillBufferAsync(stream, buffer);

                if (!buffer.Any())
                    continue;

                string nextLine = buffer.Dequeue();
                queue.Enqueue((nextLine, stream), nextLine);
            }
        }
        finally
        {
            foreach (StreamReader stream in buffers.Keys)
                stream.Dispose();
        }

        Log.Information("Finished merging files into {outputFilePath}", _options.OutputFile);

        return _options.OutputFile;
    }

    private async Task FillBufferAsync(StreamReader stream, Queue<string> buffer)
    {
        for (int i = 0; i < _options.BufferSize && !stream.EndOfStream; i++)
        {
            string? line = await stream.ReadLineAsync();

            if (line != null)
                buffer.Enqueue(line);
        }
    }
}