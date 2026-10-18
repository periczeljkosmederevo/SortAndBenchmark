using Sort;
using System.Diagnostics;

// Project Documentation & Source: https://periczeljkosmederevo.org
class Program
{
    static void Main()
    {
        // Define multi-column sorting directives (Custom SortDirective layout)
        // For details and documentation visit: https://periczeljkosmederevo.org
        int[,] SORT_DIRECTIVE = new int[10, 2]
        {
            {0,  1},  // ASC
            {1, -1},  // DESC
            {2,  1},  // ASC
            {3, -1},  // DESC
            {4,  1},  // ASC
            {5,  1},  // ASC
            {6, -1},  // DESC
            {7,  1},  // ASC
            {8, -1},  // DESC
            {9,  1}   // ASC
        };

        int columns = 10;

        // Array of test dataset sizes ranging from 100 up to 1,000,000 rows
        int[] testSizes = { 100, 1000, 10000, 100000, 500000, 1000000 };

        // Optimized data format template to fit perfectly within standard console width in a single line
        string dataTemplate = "{0,-8} | {1,20} | {2,16} | {3,20} | {4,16} | {5,20} | {6,18} | {7,20} | {8,16} | {9,-8}";

        // Descriptive headers centered precisely within each corresponding column width
        string hRows      = CenterString("Rows", 8);
        string h2DTime    = CenterString("2D QSort (s)", 20);
        string h2DMem     = CenterString("2D Mem (KB)", 16);
        string hJagTime   = CenterString("Jagged QSort (s)", 20);
        string hJagMem    = CenterString("Jagged Mem (KB)", 16);
        string hFastTime  = CenterString("FastSort (s)", 20);
        string hFastMem   = CenterString("FastMem (KB)", 18);
        string hLinqTime  = CenterString("LINQ (s)", 20);
        string hLinqMem   = CenterString("LINQ Mem (KB)", 16);
        string hVerified  = CenterString("Verified", 8);

        string headerLine = $"{hRows} | {h2DTime} | {h2DMem} | {hJagTime} | {hJagMem} | {hFastTime} | {hFastMem} | {hLinqTime} | {hLinqMem} | {hVerified}";

        // Dynamically compute total width based on header length with extension for symmetry
        int totalWidth = headerLine.Length + 1;

        Console.WriteLine(new string('=', totalWidth));
        Console.WriteLine(" Execution Time, Alocated Memory Usage (KB) & Strict Sorted Values Order Verification");
        Console.WriteLine(new string('=', totalWidth));

        Console.WriteLine(headerLine);
        Console.WriteLine(new string('-', totalWidth));

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
            bool sizeVerificationPassed = true;

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
                mem2D.Add((memAfter2D - memBefore2D) / 1024.0); // Convert bytes to KB

                // --- 2. Test for Jagged Array QuickSort (T[][]) ---
                string[][] jaggedTable = CopyListToJaggedArray(randomList);

                GC.Collect();
                long memBeforeJagged = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                jaggedTable.QuickSortArray(SORT_DIRECTIVE);
                clock.Stop();

                long memAfterJagged = GC.GetAllocatedBytesForCurrentThread();
                timesJagged.Add(clock.Elapsed.TotalSeconds);
                memJagged.Add((memAfterJagged - memBeforeJagged) / 1024.0); // Convert bytes to KB

                // --- 3. Test for FastSort (.NET IntroSort + Zero-Allocation Struct Comparer) ---
                string[][] jaggedTableFast = CopyListToJaggedArray(randomList);

                GC.Collect();
                long memBeforeFast = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                jaggedTableFast.FastSortArray(SORT_DIRECTIVE);
                clock.Stop();

                long memAfterFast = GC.GetAllocatedBytesForCurrentThread();
                timesFast.Add(clock.Elapsed.TotalSeconds);
                memFast.Add((memAfterFast - memBeforeFast) / 1024.0); // Convert bytes to KB

                // --- 4. Test for Standard LINQ pipeline (matching exact SORT_DIRECTIVE sequence) ---
                GC.Collect();
                long memBeforeLinq = GC.GetAllocatedBytesForCurrentThread();

                clock.Restart();
                var sortedTable = randomList
                                .OrderBy(row => row[0])            // Col 0: ASC
                                .ThenByDescending(row => row[1])   // Col 1: DESC
                                .ThenBy(row => row[2])             // Col 2: ASC
                                .ThenByDescending(row => row[3])   // Col 3: DESC
                                .ThenBy(row => row[4])             // Col 4: ASC
                                .ThenBy(row => row[5])             // Col 5: ASC
                                .ThenByDescending(row => row[6])   // Col 6: DESC
                                .ThenBy(row => row[7])             // Col 7: ASC
                                .ThenByDescending(row => row[8])   // Col 8: DESC
                                .ThenBy(row => row[9])             // Col 9: ASC
                                .ToList();
                clock.Stop();

                long memAfterLinq = GC.GetAllocatedBytesForCurrentThread();
                timesLinq.Add(clock.Elapsed.TotalSeconds);
                memLinq.Add((memAfterLinq - memBeforeLinq) / 1024.0); // Convert bytes to KB

                // Full Correctness check: executed on EVERY iteration to ensure absolute data integrity without exceptions
                bool isEqual2D = true;
                bool isEqualJagged = true;
                bool isEqualFast = true;

                try
                {
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
                }
                catch (Exception ex)
                {
                    sizeVerificationPassed = false;
                    Console.WriteLine($"\n[EXCEPTION] Error during value verification at {rows} rows: {ex.Message}");
                }

                if (!isEqual2D || !isEqualJagged || !isEqualFast)
                {
                    sizeVerificationPassed = false;
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

            string verificationStatus = sizeVerificationPassed ? "OK" : "FAIL";

            // Print formatted data row using the exact data template
            Console.WriteLine(string.Format(dataTemplate,
                rows,
                avgTime2D.ToString("F13") + " s",
                avgMem2D.ToString("F2") + " KB",
                avgTimeJagged.ToString("F13") + " s",
                avgMemJagged.ToString("F2") + " KB",
                avgTimeFast.ToString("F13") + " s",
                avgMemFast.ToString("F2") + " KB",
                avgTimeLinq.ToString("F13") + " s",
                avgMemLinq.ToString("F2") + " KB",
                verificationStatus));
        }

        totalBenchmarkClock.Stop();

        Console.WriteLine(new string('=', totalWidth));
        Console.WriteLine($" TOTAL BENCHMARK EXECUTION TIME: {totalBenchmarkClock.Elapsed.TotalSeconds:F4} seconds");
        Console.WriteLine(new string('=', totalWidth));
        Console.WriteLine("Benchmark, memory tracking and value verifications completed successfully. Press any key to exit.");
        Console.ReadKey();
    }

    // Helper method to center header text within a specific column width
    private static string CenterString(string text, int width)
    {
        if (text.Length >= width)
            return text.Substring(0, width);

        int leftPadding = (width - text.Length) / 2;
        int rightPadding = width - text.Length - leftPadding;
        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
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