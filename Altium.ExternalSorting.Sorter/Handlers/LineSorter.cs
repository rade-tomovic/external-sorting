using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineSorter
{
    public string[] SortLines(IEnumerable<string>? lines)
    {
        if (lines != null)
            return lines.OrderBy(line => line, new LineComparer()).ToArray();

        Log.Warning("The lines parameter is null.");

        return [];
    }
}
