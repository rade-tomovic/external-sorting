using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineSorter : ILineSorter
{
    public IEnumerable<string> SortLines(IEnumerable<string>? lines, IComparer<string> comparer)
    {
        if (lines != null)
        {
            var lineList = lines.ToList();
            lineList.Sort(comparer);
            return lineList;
        }

        Log.Warning("The lines parameter is null.");

        return [];
    }
}