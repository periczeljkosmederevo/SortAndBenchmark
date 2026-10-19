using System.Runtime.CompilerServices;

namespace Sort
{
    /// <summary>
    /// Specifies the direction of a sort operation.
    /// </summary>
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Represents a single sorting rule defining the target column index and sort order.
    /// </summary>
    public readonly struct SortDirective(int column, SortOrder order)
    {
        public int Column { get; } = column;
        public SortOrder Order { get; } = order;
    }

    /// <summary>
    /// Provides high-performance extension methods for sorting multi-dimensional (2D) 
    /// and jagged arrays using advanced sorting algorithms and custom multi-column directives.
    /// </summary>
    public static class SortExtensions
    {
        #region Fast Sort (Jagged Arrays)

        /// <summary>
        /// Sorts a jagged array using the .NET IntroSort algorithm paired with a zero-allocation 
        /// struct comparer. This method provides maximum performance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array, must implement IComparable{T}.</typeparam>
        /// <param name="array">The jagged array of rows to be sorted.</param>
        /// <param name="directives">The sorting directives specifying column indices and sort orders.</param>
        public static void FastSortArray<T>(this T[][] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Guard clause: Exit early if the array is null, has 1 or fewer rows, or no directives
            if (array == null || array.Length <= 1 || directives == null || directives.Length == 0)
                return;

            // Invoke optimized .NET Array.Sort (IntroSort) with a zero-allocation struct comparer
            Array.Sort(array, new JaggedRowComparer<T>(directives));
        }

        #endregion

        #region Quick Sort for Jagged Arrays (High Performance)

        /// <summary>
        /// Sorts a jagged array based on multi-column sort directives using the QuickSort algorithm.
        /// Swaps row references instead of copying elements individually, offering high performance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The jagged array to sort.</param>
        /// <param name="directives">The sorting rules specifying column index and order.</param>
        public static void QuickSortArray<T>(this T[][] array, params SortDirective[] directives)
            where T : IComparable<T>
        {
            // Exit if the array is empty, has a single row, or no directives
            if (array == null || array.Length <= 1 || directives == null || directives.Length == 0)
                return;

            // Start the recursive QuickSort partitioning process for jagged arrays
            QuickSortJagged(array, directives, 0, array.Length - 1);
        }

        // Core recursive QuickSort method for jagged arrays
        private static void QuickSortJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            // Continue sorting only if the segment has more than one element
            if (low < high)
            {
                // Partition the array around a pivot and get its final sorted index
                int pi = PartitionJagged(array, directives, low, high);

                // Recursively sort elements before and after the partition index
                QuickSortJagged(array, directives, low, pi - 1);
                QuickSortJagged(array, directives, pi + 1, high);
            }
        }

        // Partitions the jagged array segment using the last row as a pivot
        private static int PartitionJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            // Select the last row as the pivot reference
            T[] pivot = array[high];
            int i = low - 1; // Index of smaller element

            // Scan through the segment and rearrange rows based on multi-column rules
            for (int j = low; j < high; j++)
            {
                // Check if current row should precede the pivot
                if (IsBeforeJagged(array[j], pivot, directives))
                {
                    i++;
                    SwapJaggedRows(array, i, j); // Swap rows via reference pointers
                }
            }

            // Place the pivot into its correct sorted position
            SwapJaggedRows(array, i + 1, high);
            return i + 1; // Return the partition index
        }

        // Evaluates multi-column sorting precedence between a row and the pivot
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
            // If all columns are identical, keep original relative order
            return true;
        }

        // Extremely fast row swapping via tuple assignment (no cell copying)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapJaggedRows<T>(T[][] array, int i, int j)
        {
            (array[j], array[i]) = (array[i], array[j]);
        }

        #endregion

        #region Quick Sort for Traditional 2D Arrays (T[,])

        /// <summary>
        /// Sorts a traditional two-dimensional array based on sort directives using QuickSort.
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

        // Recursive helper for 2D array QuickSort
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

        // Partitions a traditional 2D array by copying the pivot row into a temporary 1D buffer
        private static int Partition2D<T>(T[,] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            int cols = array.GetLength(1);
            T[] pivot = new T[cols];

            // Extract the pivot row from the 2D array
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
                    SwapRows2D(array, i, j); // Physically swaps cells column by column
                }
            }

            SwapRows2D(array, i + 1, high);
            return i + 1;
        }

        // Evaluates sorting precedence for 2D array elements
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

        // Physically swaps all individual elements across columns for two rows in a 2D array
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
        /// Sorts a traditional two-dimensional array using the Selection Sort algorithm 
        /// based on multi-column sorting directives.
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
        /// A high-performance, zero-allocation struct-based comparer for jagged array rows.
        /// Enables JIT compiler devirtualization and inlining for maximum execution speed.
        /// </summary>
        private readonly struct JaggedRowComparer<T>(SortDirective[] directives) : IComparer<T[]>
            where T : IComparable<T>
        {
            // Custom comparison logic executed millions of times during sorting
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T[]? rowA, T[]? rowB)
            {
                // Handle null reference edge cases
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

                    // Return comparison outcome adjusted by sort direction
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