// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Altium.ExternalSorting.Runner;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting application");
    var externalSortingClient = new ExternalSortingClient();

    var stopwatch = Stopwatch.StartNew();
    await externalSortingClient.SortFileAsync("tmpe1khxr.txt", "output.txt");
    stopwatch.Stop();

    Log.Information("Elapsed time: {Elapsed}", stopwatch.Elapsed);
    Log.Information("Total memory: {TotalMemory} bytes", GC.GetTotalMemory(false));

    Log.Information("Application has finished successfully");
    Console.ReadLine();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}