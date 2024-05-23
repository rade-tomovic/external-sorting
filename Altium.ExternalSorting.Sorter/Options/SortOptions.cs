namespace Altium.ExternalSorting.Sorter.Options;

public record SortOptions : OptionsBase
{
    public IComparer<string> Comparer { get; init; } = Comparer<string>.Default;
    public int IncomingBufferSize { get; init; } = 1048576;
    public int OutgoingBufferSize { get; init; } = 1048576;
}