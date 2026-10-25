using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using SortAndBenchmark;

class Program
{
    static void Main(string[] args)
    {
        // Display startup console messages
        Console.WriteLine("======================================================================");
        Console.WriteLine(" STARTING BENCHMARK EXECUTION: LINQ vs Custom Sorting Algorithms");
        Console.WriteLine("======================================================================");
        Console.WriteLine("Initializing BenchmarkDotNet environment. Please wait...\n");

        // Configure BenchmarkDotNet to export results into multiple file formats
        var config = DefaultConfig.Instance
            .AddExporter(MarkdownExporter.Default); // Export results to Markdown file

        // Run the benchmark suite with the specified configuration
        BenchmarkRunner.Run<Benchmark>(config);

        // Exit handling: keep console open until user presses a key
        Console.WriteLine("\nBenchmark execution finished successfully. Press any key to exit.");
        Console.ReadKey();

        Console.WriteLine("\nConfirm exiting console. Press any key to exit.");
        Console.ReadKey();
    }
}
