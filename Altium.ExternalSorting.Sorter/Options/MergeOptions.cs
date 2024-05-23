namespace Altium.ExternalSorting.Sorter.Options;

public record MergeOptions : OptionsBase
{
    public int NumberOfFilesPerRound { get; init; } = 10;
    public int IncomingBufferSize { get; init; } = 1048576;
    public int OutgoingBufferSize { get; init; } = 1048576;
}