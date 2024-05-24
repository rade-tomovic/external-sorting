# Altium.ExternalSorting

This project provides an implementation of external sorting. It is designed to sort large files that do not fit into memory.

## How it works

The main entry point of the application is `Program.cs`. It initializes the logger, gets the input and output file paths from the user, and sets up the options for the sorting process.

The sorting process is handled by the `ExternalSortingClient` class. It takes in the `RunnerOptions` which includes the input file path, split options, sort options, and merge options.

The `ExternalSortingClient` uses a `FileSplitHandler` to split the input file into smaller chunks that fit into memory. Each time a chunk is written to a file, an event is triggered which initiates a sorting task for that file.

The sorting of each chunk is handled by the `FileSortHandler`. It sorts the file and returns the path to the sorted file.

Once all chunks are sorted, a `FileMergeHandler` merges all sorted chunks into a final sorted file.

## Usage

1. Run the application.
2. When prompted, enter the path to the input file that you want to sort.
3. Enter the path to the output file where the sorted data should be written. The file should have a `.txt` extension.
4. The application will then sort the file and write the sorted data to the output file. The elapsed time will be logged to the console.

## Note

This application uses Serilog for logging. The logs are written to the console.