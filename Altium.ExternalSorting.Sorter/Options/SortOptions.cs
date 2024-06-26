﻿using Altium.ExternalSorting.Sorter.Handlers;

namespace Altium.ExternalSorting.Sorter.Options;

public record SortOptions
{
    public IComparer<string> Comparer { get; init; } = Comparer<string>.Default;
    public ILineSorter LineSorter { get; init; } = new LineSorter();
}