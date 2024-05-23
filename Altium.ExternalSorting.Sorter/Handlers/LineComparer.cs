namespace Altium.ExternalSorting.Sorter.Handlers;

public class LineComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        int xIndex = x.IndexOf(". ");
        int yIndex = y.IndexOf(". ");

        if (xIndex == -1 || yIndex == -1) return 0;

        ReadOnlySpan<char> xSpan = x.AsSpan(xIndex + 2);
        ReadOnlySpan<char> ySpan = y.AsSpan(yIndex + 2);

        int stringComparison = xSpan.CompareTo(ySpan, StringComparison.Ordinal);

        if (stringComparison != 0) return stringComparison;

        if (int.TryParse(x.AsSpan(0, xIndex), out int xNumber) && int.TryParse(y.AsSpan(0, yIndex), out int yNumber))
        {
            return xNumber.CompareTo(yNumber);
        }

        return 0;
    }
}