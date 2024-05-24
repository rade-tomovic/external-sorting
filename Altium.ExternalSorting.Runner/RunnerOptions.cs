using Altium.ExternalSorting.Sorter.Options;

namespace Altium.ExternalSorting.Runner;

public record RunnerOptions
{
    public string InputFilePath { get; init; } = string.Empty;
    public SortOptions SortOptions { get; init; } = new();
    public SplitOptions SplitOptions { get; init; } = new();
    public MergeOptions MergeOptions { get; init; } = new();

};