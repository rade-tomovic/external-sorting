using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null || y == null)
        {
            Log.Warning("One or both of the strings being compared are null.");
            return 0;
        }

        string[] xParts = x.Split(". ", 2);
        string[] yParts = y.Split(". ", 2);

        if (xParts.Length < 2 || yParts.Length < 2)
        {
            Log.Warning("One or both of the strings being compared do not contain a '. ' separator.");
            return 0;
        }

        int stringComparison = string.Compare(xParts[1], yParts[1], StringComparison.Ordinal);

        return stringComparison != 0 ?
            stringComparison :
            int.Parse(xParts[0]).CompareTo(int.Parse(yParts[0]));
    }
}
