namespace Altium.ExternalSorting.Sorter.Options;

public record MergeOptions
{
    public int BufferSize { get; init; } = 1024;
    public string? OutputFile { get; init; } = string.Empty;
}