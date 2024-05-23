using Altium.ExternalSorting.Sorter.Handlers;
using Serilog;

public class FileMergeHandler
{
    private const int BufferSize = 1024;

    public async Task<string> MergeFilesAsync(IReadOnlyCollection<string> filePaths, string outputFilePath)
    {
        PriorityQueue<(string Line, StreamReader Stream), string> queue = new(new LineComparer());
        Dictionary<StreamReader, Queue<string>> buffers = new();

        Log.Information("Starting to merge {count} files into {outputFilePath}", filePaths.Count, outputFilePath);

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

            await using var outputStream = new StreamWriter(outputFilePath);

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

        Log.Information("Finished merging files into {outputFilePath}", outputFilePath);

        return outputFilePath;
    }

    private static async Task FillBufferAsync(StreamReader stream, Queue<string> buffer)
    {
        for (int i = 0; i < BufferSize && !stream.EndOfStream; i++)
        {
            string? line = await stream.ReadLineAsync();

            if (line != null)
                buffer.Enqueue(line);
        }
    }
}