namespace SortAndBenchmark
{
    /// <summary>
    /// Specifies the sorting direction for individual columns (Ascending or Descending).
    /// </summary>
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Represents a single multi-column sorting rule, pairing a target column index with its desired sort order.
    /// Implemented as a readonly struct using C# primary constructors for absolute immutability and zero overhead.
    /// </summary>
    public readonly struct SortDirective(int column, SortOrder order)
    {
        /// <summary>
        /// Gets the zero-based index of the target column to sort.
        /// </summary>
        public int Column { get; } = column;

        /// <summary>
        /// Gets the sorting order direction (Ascending or Descending).
        /// </summary>
        public SortOrder Order { get; } = order;
    }

    #region Fluent API Builder & Extensions

    /// <summary>
    /// Provides a fluent builder pattern (<c>SortBuilder&lt;T&gt;</c>) enabling LINQ-like method chaining 
    /// (<c>OrderBy</c>, <c>ThenByDescending</c>, etc.) for constructing multi-column sorting rules intuitively.
    /// </summary>
    public class SortBuilder<T> where T : IComparable<T>
    {
        private readonly List<SortDirective> _directives = [];

        /// <summary>
        /// Adds a primary sorting rule in ascending order (default LINQ-like behavior).
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> OrderBy(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Ascending));
            return this;
        }

        /// <summary>
        /// Adds a primary sorting rule in ascending order (default LINQ-like behavior).
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> ThenBy(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Ascending));
            return this;
        }

        /// <summary>
        /// Adds a primary sorting rule in explicit ascending order.
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> OrderByAscending(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Ascending));
            return this;
        }

        /// <summary>
        /// Adds a primary sorting rule in explicit descending order.
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> OrderByDescending(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Descending));
            return this;
        }

        /// <summary>
        /// Adds a secondary or subsequent sorting rule in ascending order.
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> ThenByAscending(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Ascending));
            return this;
        }

        /// <summary>
        /// Adds a secondary or subsequent sorting rule in descending order.
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The current sort builder instance to allow fluent method chaining.</returns>
        public SortBuilder<T> ThenByDescending(int column)
        {
            _directives.Add(new SortDirective(column, SortOrder.Descending));
            return this;
        }

        /// <summary>
        /// Compiles and returns an array of accumulated sorting directives.
        /// </summary>
        /// <returns>An array containing all configured sorting rules.</returns>
        public SortDirective[] Build() => [.. _directives];
    }

    /// <summary>
    /// Provides fluent extension methods that accept a lambda configurator to streamline sorting execution.
    /// </summary>
    public static class FluentSortExtensions
    {
        /// <summary>
        /// Sorts a jagged array (<c>T[][]</c>) fluently using high-performance FastSort (IntroSort + zero-allocation comparer).
        /// </summary>
        public static T[][]? FastSortRows<T>(this T[][] array, Action<SortBuilder<T>> ruleConfigurator)
            where T : IComparable<T>
        {
            if (array == null || array.Length <= 1)
                return array;

            var builder = new SortBuilder<T>();
            ruleConfigurator(builder);

            array.FastSortArray(builder.Build());
            return array;
        }

        /// <summary>
        /// Sorts a jagged array (<c>T[][]</c>) fluently using QuickSort with reference pointer swapping.
        /// </summary>
        public static T[][]? QuickSortRows<T>(this T[][] array, Action<SortBuilder<T>> ruleConfigurator)
            where T : IComparable<T>
        {
            if (array == null || array.Length <= 1)
                return array;

            var builder = new SortBuilder<T>();
            ruleConfigurator(builder);

            array.QuickSortArray(builder.Build());
            return array;
        }

        /// <summary>
        /// Sorts a traditional 2D array (<c>T[,]</c>) fluently using QuickSort.
        /// </summary>
        public static T[,]? QuickSortRows<T>(this T[,] array, Action<SortBuilder<T>> ruleConfigurator)
            where T : IComparable<T>
        {
            if (array == null || array.GetLength(0) <= 1)
                return array;

            var builder = new SortBuilder<T>();
            ruleConfigurator(builder);

            array.QuickSortArray(builder.Build());
            return array;
        }

        /// <summary>
        /// Sorts a traditional 2D array (<c>T[,]</c>) fluently using SelectionSort.
        /// </summary>
        public static T[,]? SelectionSortRows<T>(this T[,] array, Action<SortBuilder<T>> ruleConfigurator)
            where T : IComparable<T>
        {
            if (array == null || array.GetLength(0) <= 1)
                return array;

            var builder = new SortBuilder<T>();
            ruleConfigurator(builder);

            array.SelectionSortArray(builder.Build());
            return array;
        }
    }

    #endregion

    /// <summary>
    /// Provides high-performance extension methods for sorting multi-dimensional (2D) and jagged arrays 
    /// using advanced sorting strategies and custom multi-column directives.
    /// </summary>
    public static class SortExtensions
    {
        #region Fast Sort (Jagged Arrays)

        /// <summary>
        /// Sorts a jagged array using .NET's native IntroSort algorithm paired with a zero-allocation struct comparer.
        /// This method achieves maximum performance and minimal GC pressure.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array, must implement IComparable{T}.</typeparam>
        /// <param name="array">The jagged array of rows to be sorted.</param>
        /// <param name="directives">The sorting directives specifying column indices and sort orders.</param>
        public static void FastSortArray<T>(this T[][] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Guard clause: Return early if the array is null, has 1 or fewer rows, or no directives are provided.
            if (array == null || array.Length <= 1 || directives == null || directives.Length == 0)
                return;

            // Invoke optimized .NET Array.Sort utilizing our custom zero-allocation struct comparer.
            Array.Sort(array, new JaggedRowComparer<T>(directives));
        }

        #endregion

        #region Quick Sort for Jagged Arrays (High Performance)

        /// <summary>
        /// Sorts a jagged array based on multi-column sort directives using a custom QuickSort implementation.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The jagged array to sort.</param>
        /// <param name="directives">The sorting rules specifying column index and order.</param>
        public static void QuickSortArray<T>(this T[][] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Guard clause: Ensure sorting is necessary.
            if (array == null || array.Length <= 1 || directives == null || directives.Length == 0)
                return;

            // Initiate the recursive QuickSort partitioning process across the entire row span.
            QuickSortJagged(array, directives, 0, array.Length - 1);
        }

        /// <summary>
        /// Core recursive QuickSort method for sorting segments of jagged array rows.
        /// </summary>
        private static void QuickSortJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            // Continue sorting only if the segment has more than one element
            if (low < high)
            {
                // Partition the array segment around a pivot and retrieve its final sorted index.
                int pi = PartitionJagged(array, directives, low, high);

                // Recursively sort elements before and after the partition index.
                QuickSortJagged(array, directives, low, pi - 1);
                QuickSortJagged(array, directives, pi + 1, high);
            }
        }

        /// <summary>
        /// Partitions a jagged array segment using the last row as the pivot reference.
        /// </summary>
        private static int PartitionJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            T[] pivot = array[high];
            int i = low - 1; // Index of smaller element

            // Scan through the segment and rearrange rows based on multi-column rules
            for (int j = low; j < high; j++)
            {
                // Check if current row should precede the pivot
                if (IsBeforeJagged(array[j], pivot, directives))
                {
                    i++;
                    SwapJaggedRows(array, i, j); // Swap row reference pointers directly.
                }
            }

            // Place the pivot row into its correct sorted position.
            SwapJaggedRows(array, i + 1, high);
            return i + 1;
        }

        /// <summary>
        /// Evaluates multi-column sorting precedence between a target row and a pivot row.
        /// Marked with AggressiveInlining to allow JIT compiler devirtualization and inline optimization.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBeforeJagged<T>(T[] row, T[] pivot, SortDirective[] directives)
            where T : IComparable<T>
        {
            // Iterate through each sorting directive sequentially (primary, secondary, etc.)
            for (int i = 0; i < directives.Length; i++)
            {
                ref readonly var directive = ref directives[i];
                int col = directive.Column;
                SortOrder order = directive.Order;

                // Compare individual cell values at the target column
                int comparison = row[col].CompareTo(pivot[col]);

                // If values are not equal, determine precedence based on sort order
                if (comparison != 0)
                {
                    return order == SortOrder.Ascending ? comparison < 0 : comparison > 0;
                }
            }
            // Fallback if all compared columns are identical.
            return true;
        }

        /// <summary>
        /// Swaps two rows in a jagged array instantly via tuple assignment without copying individual cells.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapJaggedRows<T>(T[][] array, int i, int j)
        {
            (array[j], array[i]) = (array[i], array[j]);
        }

        #endregion

        #region Quick Sort for Traditional 2D Arrays (T[,])

        /// <summary>
        /// Sorts a traditional two-dimensional rectangular array based on sort directives using QuickSort.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The 2D array to sort.</param>
        /// <param name="directives">The sorting directives.</param>
        public static void QuickSortArray<T>(this T[,] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Guard clause for 2D array row count or directives
            if (array == null || array.GetLength(0) <= 1 || directives == null || directives.Length == 0)
                return;

            // Initiate recursive 2D QuickSort
            QuickSort2D(array, directives, 0, array.GetLength(0) - 1);
        }

        /// <summary>
        /// Recursive helper method for 2D array QuickSort operations.
        /// </summary>
        private static void QuickSort2D<T>(T[,] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            if (low < high)
            {
                int pi = Partition2D(array, directives, low, high);

                QuickSort2D(array, directives, low, pi - 1);
                QuickSort2D(array, directives, pi + 1, high);
            }
        }

        /// <summary>
        /// Partitions a rectangular 2D array by extracting the pivot row into a temporary buffer 
        /// and physically swapping columns during rearrangement.
        /// </summary>
        private static int Partition2D<T>(T[,] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            int cols = array.GetLength(1);
            T[] pivot = new T[cols];

            // Extract the pivot row values into a 1D temporary buffer.
            for (int k = 0; k < cols; k++)
            {
                pivot[k] = array[high, k];
            }

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (IsBefore2D(array, directives, j, pivot))
                {
                    i++;
                    SwapRows2D(array, i, j);
                }
            }

            SwapRows2D(array, i + 1, high);
            return i + 1;
        }

        /// <summary>
        /// Evaluates multi-column sorting precedence for rectangular 2D array rows.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBefore2D<T>(T[,] array, SortDirective[] directives, int row, T[] pivot)
            where T : IComparable<T>
        {
            for (int i = 0; i < directives.Length; i++)
            {
                ref readonly var directive = ref directives[i];
                int column = directive.Column;
                SortOrder order = directive.Order;

                int comparison = array[row, column].CompareTo(pivot[column]);

                if (comparison != 0)
                {
                    if ((order == SortOrder.Ascending && comparison > 0) || (order == SortOrder.Descending && comparison < 0))
                    {
                        return false;
                    }
                    else if ((order == SortOrder.Descending && comparison > 0) || (order == SortOrder.Ascending && comparison < 0))
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Physically swaps all individual elements across columns for two rows in a rectangular 2D array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRows2D<T>(T[,] array, int i, int j)
        {
            int cols = array.GetLength(1);
            for (int col = 0; col < cols; col++)
            {
                (array[j, col], array[i, col]) = (array[i, col], array[j, col]);
            }
        }

        #endregion

        #region Selection Sort for Traditional 2D Arrays (T[,])

        /// <summary>
        /// Sorts a traditional two-dimensional rectangular array using the Selection Sort algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The 2D array to sort.</param>
        /// <param name="directives">The sorting directives.</param>
        public static void SelectionSortArray<T>(this T[,] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Guard clause for null or single-row 2D arrays
            if (array == null || array.GetLength(0) <= 1 || directives == null || directives.Length == 0)
                return;

            int arrayRows = array.GetLength(0);
            int arrayColumns = array.GetLength(1);
            int sortDirectiveCount = directives.Length;

            // Selection sort outer loop
            for (int i = 0; i < arrayRows - 1; i++)
            {
                int minIndex = i;

                // Inner loop to find the extreme element according to multi-column rules
                for (int j = i + 1; j < arrayRows; j++)
                {
                    bool shouldSwap = false;

                    for (int c = 0; c < sortDirectiveCount; c++)
                    {
                        ref readonly var directive = ref directives[c];
                        int columnToSort = directive.Column;
                        SortOrder sortOrder = directive.Order;

                        T valueI = array[minIndex, columnToSort];
                        T valueJ = array[j, columnToSort];

                        int comparisonResult = valueI.CompareTo(valueJ);

                        int compRule = sortOrder == SortOrder.Descending ? -comparisonResult : comparisonResult;
                        if (compRule < 0)
                        {
                            shouldSwap = true;
                            break;
                        }
                        else if (compRule > 0)
                        {
                            break;
                        }
                    }

                    if (shouldSwap)
                    {
                        minIndex = j;
                    }
                }

                // Swap rows if a new minimum/maximum row was found
                if (minIndex != i)
                {
                    for (int d = 0; d < arrayColumns; d++)
                    {
                        (array[i, d], array[minIndex, d]) = (array[minIndex, d], array[i, d]);
                    }
                }
            }
        }

        #endregion

        #region Shared Supporting Structures

        /// <summary>
        /// A high-performance, zero-allocation struct-based comparer implementing <c>IComparer&lt;T[]&gt;</c> for jagged array rows.
        /// Designed specifically to enable JIT compiler devirtualization and aggressive method inlining during sorting operations.
        /// </summary>
        private readonly struct JaggedRowComparer<T>(SortDirective[] directives) : IComparer<T[]>
            where T : IComparable<T>
        {
            /// <summary>
            /// Compares two jagged array rows sequentially across multi-column sorting directives.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T[]? rowA, T[]? rowB)
            {
                // Handle null reference edge cases safely.
                if (rowA == rowB)
                    return 0;
                if (rowA == null)
                    return -1;
                if (rowB == null)
                    return 1;

                int length = directives.Length;

                // Iterate through multi-column directives sequentially
                for (int i = 0; i < length; i++)
                {
                    ref readonly var directive = ref directives[i];
                    int col = directive.Column;
                    SortOrder order = directive.Order;

                    int comparison = rowA[col].CompareTo(rowB[col]);

                    // Return comparison outcome adjusted by the corresponding sort direction.
                    if (comparison != 0)
                    {
                        return order == SortOrder.Ascending ? comparison : -comparison;
                    }
                }
                return 0;
            }
        }

        #endregion
    }
}