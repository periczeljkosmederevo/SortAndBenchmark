using BenchmarkDotNet.Running;
using SortBenchmarks;

class Program
{
    static void Main(string[] args)
    {
        // Display elementary startup console messages
        Console.WriteLine("======================================================================");
        Console.WriteLine(" STARTING BENCHMARK EXECUTION: LINQ vs Custom Sorting Algorithms");
        Console.WriteLine("======================================================================");
        Console.WriteLine("Initializing BenchmarkDotNet environment. Please wait...\n");

        // Execute the benchmark runner pointing to our Benchmark class
        BenchmarkRunner.Run<Benchmark>();

        // Exit handling
        Console.WriteLine("\nBenchmark execution finished successfully. Press any key to exit.");
        Console.ReadKey();
    }
}