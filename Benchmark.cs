using BenchmarkDotNet.Attributes;

namespace SortAndBenchmark
{
    /// <summary>
    /// Comprehensive benchmarking suite designed to measure pure sorting performance 
    /// (execution time and memory allocations) across multiple dataset scales (from 100 to 1,000,000 rows), 
    /// comparing custom in-place sorting algorithms against the standard LINQ query pipeline baseline.
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class Benchmark
    {
        /// <summary>
        /// Parameterized dataset row counts tested automatically by BenchmarkDotNet.
        /// Covers small, medium, and large scales to observe algorithmic complexity impact ($O(N \log N)$).
        /// </summary>
        [Params(100, 1000, 10000, 100000, 500000, 1000000)]
        public int Rows { get; set; }

        private const int _columns = 10;
        private SortDirective[] _sortDirectives = null!;
        private List<List<string>> _sourceList = null!;

        // Pre-allocated collection copies for each iteration to keep copying overhead completely out of benchmark methods.
        // Separate structures are maintained to test data layout impact (rectangular 2D array vs jagged array vs LINQ lists).
        private string[,] _array2D = null!;
        private string[][] _jaggedArray = null!;
        private string[][] _fastSortArray = null!;
        private List<List<string>> _linqList = null!;

        /// <summary>
        /// Initializes global benchmarking configurations and multi-column sorting directives.
        /// Executed once before the benchmark runs begin.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            // Define strict multi-column sorting rules matching all 10 columns with alternating directions (ASC/DESC)
            _sortDirectives =
            [
                new SortDirective(0, SortOrder.Ascending),  // Col 0: ASC
                new SortDirective(1, SortOrder.Descending), // Col 1: DESC
                new SortDirective(2, SortOrder.Ascending),  // Col 2: ASC
                new SortDirective(3, SortOrder.Descending), // Col 3: DESC
                new SortDirective(4, SortOrder.Ascending),  // Col 4: ASC
                new SortDirective(5, SortOrder.Ascending),  // Col 5: ASC
                new SortDirective(6, SortOrder.Descending), // Col 6: DESC
                new SortDirective(7, SortOrder.Ascending),  // Col 7: ASC
                new SortDirective(8, SortOrder.Descending), // Col 8: DESC
                new SortDirective(9, SortOrder.Ascending)   // Col 9: ASC
            ];
        }

        /// <summary>
        /// Generates fresh random data and populates dedicated test arrays prior to each iteration.
        /// The execution time and allocation overhead of this method are strictly excluded from benchmark measurements.
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _sourceList = RandomListGenerator.GenerateRandomList(Rows, _columns);

            // Populate test arrays outside the benchmark scope to ensure isolated sorting measurement
            _array2D = CopyListToArray(_sourceList);
            _jaggedArray = CopyListToJaggedArray(_sourceList);
            _fastSortArray = CopyListToJaggedArray(_sourceList);

            // Deep copy for LINQ to preserve source state and account for LINQ materialization overhead
            _linqList = new List<List<string>>(_sourceList.Count);
            for (int i = 0; i < _sourceList.Count; i++)
            {
                _linqList.Add(item: new List<string>(_sourceList[i]));
            }
        }

        /// <summary>
        /// Benchmarks traditional two-dimensional array (T[,]) QuickSort implementation.
        /// Measures pure in-place sorting logic and cache locality characteristics of rectangular arrays.
        /// </summary>
        [Benchmark]
        public void Benchmark_2D_QuickSort()
        {
            _array2D.QuickSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks jagged array (T[][]) QuickSort implementation with reference pointer swapping.
        /// Measures sorting performance where row references are swapped rather than deep data elements.
        /// </summary>
        [Benchmark]
        public void Benchmark_Jagged_QuickSort()
        {
            _jaggedArray.QuickSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks high-performance FastSort using .NET IntroSort combined with a 
        /// zero-allocation struct comparer. Measures optimized multi-column sorting logic with minimal GC pressure.
        /// </summary>
        [Benchmark]
        public void Benchmark_FastSort()
        {
            _fastSortArray.FastSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks standard LINQ query pipeline with multi-column OrderBy/ThenBy sorting. 
        /// Designated as the performance baseline (Baseline = true) for relative comparison 
        /// and highlights LINQ's additional memory allocations due to object wrapping, delegates, and materialization.
        /// </summary>
        [Benchmark(Baseline = true)]
        public List<List<string>> Benchmark_LINQ()
        {
            return _linqList
                .OrderBy(row => row[0])
                .ThenByDescending(row => row[1])
                .ThenBy(row => row[2])
                .ThenByDescending(row => row[3])
                .ThenBy(row => row[4])
                .ThenBy(row => row[5])
                .ThenByDescending(row => row[6])
                .ThenBy(row => row[7])
                .ThenByDescending(row => row[8])
                .ThenBy(row => row[9])
                .ToList();
        }

        #region Helper Methods for Data Transformation

        // Converts standard list representation into a traditional 2D array (T[,])
        private static string[,] CopyListToArray(List<List<string>> list)
        {
            int rows = list.Count;
            int cols = list[0].Count;
            string[,] array = new string[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = list[i][j];
                }
            }
            return array;
        }

        // Converts standard list representation into a jagged array (T[][])
        private static string[][] CopyListToJaggedArray(List<List<string>> list)
        {
            int rows = list.Count;
            string[][] array = new string[rows][];

            for (int i = 0; i < rows; i++)
            {
                array[i] = [.. list[i]];
            }
            return array;
        }

        #endregion
    }
}