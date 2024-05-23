namespace Altium.ExternalSorting.Sorter.Options;

public record OptionsBase
{
    public IProgress<double> ProgressHandler { get; init; } = null!;
}