using Altium.ExternalSorting.Sorter.Handlers;
using FluentAssertions;

namespace Altium.ExternalSorting.Sorter.Tests;

public class LineSorterTests
{
    private readonly LineSorter _lineSorter = new();

    [Fact]
    public void SortLines_WhenCalledWithNull_ShouldReturnEmptyArray()
    {
        IEnumerable<string> result = _lineSorter.SortLines(null, new LineComparer());

        result.Should().BeEmpty();
    }

    [Fact]
    public void SortLines_WhenCalledWithUnsortedLines_ShouldReturnSortedLines()
    {
        string[] unsortedLines = [
            "415. Apple",
            "30432. Something something something",
            "1. Apple",
            "32. Cherry is the best",
            "2. Banana is yellow"
        ];
        string[] expectedSortedLines = [
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        ];

        IEnumerable<string> result = _lineSorter.SortLines(unsortedLines, new LineComparer());

        result.Should().Equal(expectedSortedLines);
    }

    [Fact]
    public void SortLines_WhenCalledWithSameStringDifferentNumbersAndSameNumbersDifferentStrings_ShouldReturnSortedLines()
    {
        string[] unsortedLines = [
            "10. Apple",
            "2. Apple",
            "1000. Banana",
            "20. Banana"
        ];
        string[] expectedSortedLines = [
            "2. Apple",
            "10. Apple",
            "20. Banana",
            "1000. Banana"
        ];

        IEnumerable<string> result = _lineSorter.SortLines(unsortedLines, new LineComparer());

        result.Should().Equal(expectedSortedLines);
    }
}