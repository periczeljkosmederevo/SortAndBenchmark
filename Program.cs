using Sort;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // Define multi-column sorting directives (10 columns, alternating ASC and DESC)
        int[,] SORT_DIRECTIVE = new int[10, 2]
        {
            {0,  1},
            {1, -1},
            {2,  1},
            {3, -1},
            {4,  1},
            {5,  1},
            {6, -1},
            {7,  1},
            {8, -1},
            {9,  1}
        };

        int columns = 10;

        // Array of test dataset sizes ranging from 100 up to 1,000,000 rows
        int[] testSizes = { 100, 1000, 10000, 100000, 500000, 1000000 };

        Console.WriteLine("==================================================================================================================================================");
        Console.WriteLine(" AUTOMATED BENCHMARK: Time & Memory Usage Verification");
        Console.WriteLine("==================================================================================================================================================");

        // Updated header with Memory column for each approach
        Console.WriteLine($"{"Rows",-10} | {"2D Time (s)",21} | {"2D Mem (MB)",14} | {"Jagged Time (s)",21} | {"Jagged Mem (MB)",16} | {"Fast Time (s)",21} | {"Fast Mem (MB)",14} | {"LINQ Time (s)",17} | {"LINQ Mem (MB)",14}");
        Console.WriteLine(new string('-', 168));

        // Master stopwatch to measure total benchmark execution time
        Stopwatch totalBenchmarkClock = Stopwatch.StartNew();

        // Iterate through each dataset size automatically
        foreach (int rows in testSizes)
        {
            List<double> times2D = new List<double>();
            List<double> timesJagged = new List<double>();
            List<double> timesFast = new List<double>();
            List<double> timesLinq = new List<double>();

            List<double> mem2D = new List<double>();
            List<double> memJagged = new List<double>();
            List<double> memFast = new List<double>();
            List<double> memLinq = new List<double>();

            Stopwatch clock = new Stopwatch();

            // Run 10 iterations per size to gather robust performance metrics
            for (int i = 0; i < 10; i++)
            {
                var randomList = RandomListGenerator.GenerateRandomList(rows, columns);

                // --- 1. Test for traditional 2D array (T[,]) QuickSort ---
                string[,] randomTable = CopyListToArray(randomList);

                GC.Collect(); // Clean slate before measurement
                long memBefore2D = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                randomTable.QuickSortArray(SORT_DIRECTIVE);
                clock.Stop();

                long memAfter2D = GC.GetAllocatedBytesForCurrentThread();
                times2D.Add(clock.Elapsed.TotalSeconds);
                mem2D.Add((memAfter2D - memBefore2D) / (1024.0 * 1024.0)); // Convert bytes to MB

                // --- 2. Test for Jagged Array QuickSort (T[][]) ---
                string[][] jaggedTable = CopyListToJaggedArray(randomList);

                GC.Collect();
                long memBeforeJagged = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                jaggedTable.QuickSortArray(SORT_DIRECTIVE);
                clock.Stop();

                long memAfterJagged = GC.GetAllocatedBytesForCurrentThread();
                timesJagged.Add(clock.Elapsed.TotalSeconds);
                memJagged.Add((memAfterJagged - memBeforeJagged) / (1024.0 * 1024.0));

                // --- 3. Test for FastSort (.NET IntroSort + Zero-Allocation Struct Comparer) ---
                string[][] jaggedTableFast = CopyListToJaggedArray(randomList);

                GC.Collect();
                long memBeforeFast = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                jaggedTableFast.FastSortArray(SORT_DIRECTIVE);
                clock.Stop();

                long memAfterFast = GC.GetAllocatedBytesForCurrentThread();
                timesFast.Add(clock.Elapsed.TotalSeconds);
                memFast.Add((memAfterFast - memBeforeFast) / (1024.0 * 1024.0));

                // --- 4. Test for Standard LINQ pipeline ---
                GC.Collect();
                long memBeforeLinq = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                var sortedTable = randomList
                                .OrderBy(row => row[0])
                                .ThenByDescending(row => row[1])
                                .ThenBy(row => row[2])
                                .ThenByDescending(row => row[3])
                                .ThenBy(row => row[4])
                                .ThenByDescending(row => row[5])
                                .ThenBy(row => row[6])
                                .ThenByDescending(row => row[7])
                                .ThenBy(row => row[8])
                                .ThenByDescending(row => row[9])
                                .ToList();
                clock.Stop();

                long memAfterLinq = GC.GetAllocatedBytesForCurrentThread();
                timesLinq.Add(clock.Elapsed.TotalSeconds);
                memLinq.Add((memAfterLinq - memBeforeLinq) / (1024.0 * 1024.0));

                // Full Correctness check: executed on EVERY iteration to ensure absolute data integrity
                bool isEqual2D = true;
                bool isEqualJagged = true;
                bool isEqualFast = true;

                for (int j = 0; j < sortedTable.Count; j++)
                {
                    for (int k = 0; k < columns; k++)
                    {
                        if (sortedTable[j][k] != randomTable[j, k])
                            isEqual2D = false;
                        if (sortedTable[j][k] != jaggedTable[j][k])
                            isEqualJagged = false;
                        if (sortedTable[j][k] != jaggedTableFast[j][k])
                            isEqualFast = false;
                    }
                }

                if (!isEqual2D || !isEqualJagged || !isEqualFast)
                {
                    Console.WriteLine($"\n[ERROR] Data mismatch detected at {rows} rows on iteration {i + 1}!");
                }
            }

            // Calculate trimmed averages (discarding min and max from 10 measurements)
            double avgTime2D = CalculateTrimmedAverage(times2D);
            double avgTimeJagged = CalculateTrimmedAverage(timesJagged);
            double avgTimeFast = CalculateTrimmedAverage(timesFast);
            double avgTimeLinq = CalculateTrimmedAverage(timesLinq);

            double avgMem2D = CalculateTrimmedAverage(mem2D);
            double avgMemJagged = CalculateTrimmedAverage(memJagged);
            double avgMemFast = CalculateTrimmedAverage(memFast);
            double avgMemLinq = CalculateTrimmedAverage(memLinq);

            // Print formatted results row with 15 decimals for time and 2 decimals for memory (MB)
            Console.WriteLine($"{rows,-10} | {avgTime2D,21:F15} s | {avgMem2D,11:F2} MB | {avgTimeJagged,21:F15} s | {avgMemJagged,13:F2} MB | {avgTimeFast,21:F15} s | {avgMemFast,11:F2} MB | {avgTimeLinq,17:F15} s | {avgMemLinq,11:F2} MB");
        }

        totalBenchmarkClock.Stop();

        Console.WriteLine("==================================================================================================================================================");
        Console.WriteLine($" TOTAL BENCHMARK EXECUTION TIME: {totalBenchmarkClock.Elapsed.TotalSeconds:F4} seconds");
        Console.WriteLine("==================================================================================================================================================");
        Console.WriteLine("Benchmark and memory tracking completed successfully. Press any key to exit.");
        Console.ReadKey();
    }

    // Helper method to discard min and max values and calculate the average of the remaining 8 measurements
    private static double CalculateTrimmedAverage(List<double> times)
    {
        times.Sort();
        double sum = 0;
        for (int i = 1; i < times.Count - 1; i++)
        {
            sum += times[i];
        }
        return sum / (times.Count - 2);
    }

    private static string[,] CopyListToArray(List<List<string>> list)
    {
        int rows = list.Count;
        int columns = list[0].Count;
        string[,] array = new string[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                array[i, j] = list[i][j];
            }
        }
        return array;
    }

    private static string[][] CopyListToJaggedArray(List<List<string>> list)
    {
        int rows = list.Count;
        string[][] array = new string[rows][];

        for (int i = 0; i < rows; i++)
        {
            array[i] = list[i].ToArray();
        }
        return array;
    }
}