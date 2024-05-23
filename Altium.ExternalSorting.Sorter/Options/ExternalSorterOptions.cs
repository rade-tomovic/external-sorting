namespace Altium.ExternalSorting.Sorter.Options;

public record ExternalSorterOptions
{
    public ExternalSorterOptions(string filePath)
    {
        SplitOptions = new();
        SortOptions = new();
        MergeOptions = new();
        FilePath = filePath;
    }

    public SplitOptions SplitOptions { get; init; }
    public SortOptions SortOptions { get; init; }
    public MergeOptions MergeOptions { get; init; }
    public string FilePath { get; init; } 
}