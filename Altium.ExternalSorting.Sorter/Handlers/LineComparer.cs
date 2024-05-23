using Serilog;

namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        int xIndex = x.IndexOf(". ");
        int yIndex = y.IndexOf(". ");

        if (xIndex == -1 || yIndex == -1)
        {
            return 0;
        }

        int stringComparison = string.Compare(x.Substring(xIndex + 2), y.Substring(yIndex + 2), StringComparison.Ordinal);

        return stringComparison != 0 ?
            stringComparison :
            int.Parse(x.Substring(0, xIndex)).CompareTo(int.Parse(y.Substring(0, yIndex)));
    }
}

