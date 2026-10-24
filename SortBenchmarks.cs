using BenchmarkDotNet.Attributes;
using Sort;

namespace SortBenchmarks
{
    /// <summary>
    /// Comprehensive benchmarking suite designed to measure pure sorting performance 
    /// (execution time and memory allocations) across multiple dataset scales, 
    /// with LINQ set as the performance baseline.
    /// </summary>
    [MemoryDiagnoser]
    [RankColumn]
    public class Benchmark
    {
        // Parameterized dataset row counts tested automatically by BenchmarkDotNet.
        // Kada vratiš veće brojeve (1000, 10000...), upozorenja za MinIterationTime će potpuno nestati.
        [Params(100, 1000, 10000, 100000)]
        public int Rows { get; set; }

        private const int Columns = 10;
        private SortDirective[] _sortDirectives = null!;
        private List<List<string>> _sourceList = null!;

        // Pre-allocated collection copies for each iteration to keep copying overhead completely out of benchmark methods
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
            // Define strict multi-column sorting rules matching all 10 columns
            _sortDirectives =
            [
                new SortDirective(0, SortOrder.Ascending),    // Col 0: ASC
                new SortDirective(1, SortOrder.Descending), // Col 1: DESC
                new SortDirective(2, SortOrder.Ascending),    // Col 2: ASC
                new SortDirective(3, SortOrder.Descending), // Col 3: DESC
                new SortDirective(4, SortOrder.Ascending),    // Col 4: ASC
                new SortDirective(5, SortOrder.Ascending),    // Col 5: ASC
                new SortDirective(6, SortOrder.Descending), // Col 6: DESC
                new SortDirective(7, SortOrder.Ascending),    // Col 7: ASC
                new SortDirective(8, SortOrder.Descending), // Col 8: DESC
                new SortDirective(9, SortOrder.Ascending)     // Col 9: ASC
            ];
        }

        /// <summary>
        /// Generates fresh random data and populates dedicated test arrays prior to each iteration.
        /// The execution time of this method is excluded from the benchmark measurements.
        /// </summary>
        [IterationSetup]
        public void IterationSetup()
        {
            _sourceList = RandomListGenerator.GenerateRandomList(Rows, Columns);

            // Populate test arrays outside the benchmark scope
            _array2D = CopyListToArray(_sourceList);
            _jaggedArray = CopyListToJaggedArray(_sourceList);
            _fastSortArray = CopyListToJaggedArray(_sourceList);

            _linqList = new List<List<string>>(_sourceList.Count);
            for (int i = 0; i < _sourceList.Count; i++)
            {
                _linqList.Add(new List<string>(_sourceList[i]));
            }
        }

        /// <summary>
        /// Benchmarks traditional two-dimensional array (T[,]) QuickSort implementation.
        /// Measures pure sorting logic only.
        /// </summary>
        [Benchmark]
        public void Benchmark_2D_QuickSort()
        {
            _array2D.QuickSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks jagged array (T[][]) QuickSort implementation with reference pointer swapping.
        /// Measures pure sorting logic only.
        /// </summary>
        [Benchmark]
        public void Benchmark_Jagged_QuickSort()
        {
            _jaggedArray.QuickSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks high-performance FastSort using .NET IntroSort combined with a 
        /// zero-allocation struct comparer. Measures pure sorting logic only.
        /// </summary>
        [Benchmark]
        public void Benchmark_FastSort()
        {
            _fastSortArray.FastSortArray(_sortDirectives);
        }

        /// <summary>
        /// Benchmarks standard LINQ query pipeline. 
        /// Designated as the performance baseline (Baseline = true) for comparison.
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