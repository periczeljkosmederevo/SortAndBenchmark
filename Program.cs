using SortAndBenchmark;

class Program
{
    static void Main()
    {
        // Display startup console messages
        Console.WriteLine("======================================================================");
        Console.WriteLine(" STARTING BENCHMARK EXECUTION: LINQ vs Custom Sorting Algorithms");
        Console.WriteLine("======================================================================");
        Console.WriteLine("Initializing BenchmarkDotNet environment. Please wait...\n");

        // Configure BenchmarkDotNet with a custom artifacts path and markdown exporter
        var config = DefaultConfig.Instance
            .WithArtifactsPath("./BenchmarkDotNet.Artifacts")
            .AddExporter(MarkdownExporter.Default);

        // Run the benchmark suite with the specified configuration
        BenchmarkRunner.Run<Benchmark>(config);

        // Exit handling: keep console open until user presses a key
        Console.WriteLine("\nBenchmark execution finished successfully. Press any key to exit.");
        Console.ReadKey();

        Console.WriteLine("\nConfirm exiting console. Press any key to exit.");
        Console.ReadKey();
    }
}
