namespace Altium.ExternalSorting.Sorter.Options;

public record SplitOptions
{
    public int SplitFileSize { get; init; } = 10 * 1024 * 1024;
    public string LineSeparator { get; init; } = Environment.NewLine;
}