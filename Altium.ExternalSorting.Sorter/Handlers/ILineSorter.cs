namespace Altium.ExternalSorting.Sorter.Handlers;

public interface ILineSorter
{
    IEnumerable<string> SortLines(IEnumerable<string>? lines, IComparer<string> comparer);
}