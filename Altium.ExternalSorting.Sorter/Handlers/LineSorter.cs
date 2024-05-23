using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineSorter : ILineSorter
{
    public IEnumerable<string> SortLines(IEnumerable<string>? lines, IComparer<string> comparer)
    {
        if (lines != null)
            return lines
                .OrderBy(line => line, comparer)
                .ToArray();

        Log.Warning("The lines parameter is null.");

        return [];
    }
}
