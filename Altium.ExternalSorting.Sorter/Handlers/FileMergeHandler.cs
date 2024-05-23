using Altium.ExternalSorting.Sorter.Handlers;

public class FileMergeHandler
{
    private const int BufferSize = 1024; // Number of lines to read from each file at a time

    public async Task<string> MergeFilesAsync(IReadOnlyCollection<string> filePaths, string outputFilePath)
    {
        PriorityQueue<(string Line, StreamReader Stream), string> queue = new(new LineComparer());
        Dictionary<StreamReader, Queue<string>> buffers = new();

        try
        {
            // Initialize streams and buffers for each file
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

            // Process lines from files
            while (queue.Count > 0)
            {
                (string line, StreamReader stream) = queue.Dequeue();
                await outputStream.WriteLineAsync(line);

                Queue<string> buffer = buffers[stream];
                if (!buffer.Any()) await FillBufferAsync(stream, buffer);

                // Only enqueue an item from the buffer if it is not empty
                if (buffer.Any())
                {
                    string nextLine = buffer.Dequeue();
                    queue.Enqueue((nextLine, stream), nextLine);
                }
            }
        }
        finally
        {
            foreach (StreamReader stream in buffers.Keys) stream.Dispose();
        }

        return outputFilePath;
    }

    private static async Task FillBufferAsync(StreamReader stream, Queue<string> buffer)
    {
        for (int i = 0; i < BufferSize && !stream.EndOfStream; i++)
        {
            string? line = await stream.ReadLineAsync();
            if (line != null) buffer.Enqueue(line);
        }
    }
}