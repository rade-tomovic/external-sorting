using System.Text;
using Serilog;

namespace Altium.ExternalSorting.FileGenerator;

public class FileBuilder
{
    private string _filePath = null!;
    private long _fileSize;

    public FileBuilder WithFilePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        _filePath = filePath;

        return this;
    }

    public FileBuilder WithFileSize(long fileSize)
    {
        if (fileSize <= 0)
            throw new ArgumentException("File size must be greater than zero.", nameof(fileSize));

        _fileSize = fileSize;

        return this;
    }

    public Result Build(Func<string>? contentGenerator = null)
    {
        try
        {
            using var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);

            while (fileStream.Length < _fileSize)
            {
                var lineGenerator = new LineGenerator(contentGenerator);
                string line = lineGenerator.Build();
                byte[] bytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
                fileStream.Write(bytes, 0, bytes.Length);
            }

            return new("File created successfully.", _filePath, true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while building the file.");
            return new($"An error occurred: {ex.Message}", null, false);
        }
    }
}