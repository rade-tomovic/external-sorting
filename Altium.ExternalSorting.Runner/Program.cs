using System.Diagnostics;
using Altium.ExternalSorting.Runner;
using Altium.ExternalSorting.Sorter.Handlers;
using Altium.ExternalSorting.Sorter.Options;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting application");

    string? inputFilePath = GetInputFilePath();
    string? outputFilePath = GetOutputFilePath();
    RunnerOptions runnerOptions = InitializeRunnerOptions(inputFilePath, outputFilePath);

    if (string.IsNullOrEmpty(inputFilePath) || string.IsNullOrEmpty(outputFilePath))
        return;

    using (var externalSortingClient = new ExternalSortingClient(runnerOptions))
    {
        var stopwatch = Stopwatch.StartNew();
        string? result = await externalSortingClient.SortFileAsync();
        stopwatch.Stop();

        Log.Information("Elapsed time: {Elapsed}", stopwatch.Elapsed);
        Log.Information("Output file: {outputFile}", result);
        Log.Information("Application has finished successfully");
    }

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

return;

static string? GetInputFilePath()
{
    Console.Write("Enter input file path: ");
    string? inputFilePath = Console.ReadLine();

    if (!string.IsNullOrEmpty(inputFilePath) && File.Exists(inputFilePath))
        return inputFilePath;

    Log.Error("Input file does not exist. Please provide a valid file path.");
    return null;
}

static string? GetOutputFilePath()
{
    Console.Write("Enter output file path: ");
    string? outputFilePath = Console.ReadLine();

    if (!string.IsNullOrEmpty(outputFilePath) && Path.GetExtension(outputFilePath) == ".txt")
        return outputFilePath;

    Log.Error("Output file path is not defined or does not have a .txt extension. Please provide a valid file path.");
    return null;
}

static RunnerOptions InitializeRunnerOptions(string? inputFilePath, string? outputFilePath) =>
    new() {
        InputFilePath = inputFilePath,
        SplitOptions = new SplitOptions { SplitFileSize = 1024 * 1024 * 100, LineSeparator = "\n" },
        SortOptions = new SortOptions { Comparer = new LineComparer(), LineSorter = new LineSorter() },
        MergeOptions = new MergeOptions { OutputFile = outputFilePath, BufferSize = 1024 }
    };