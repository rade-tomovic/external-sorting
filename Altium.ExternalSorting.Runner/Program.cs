// See https://aka.ms/new-console-template for more information
using Altium.ExternalSorting.Runner;

Console.WriteLine("Hello, World!");
var externalSortingClient = new ExternalSortingClient();
await externalSortingClient.SortFileAsync("tmpe1khxr.txt", "output.txt");