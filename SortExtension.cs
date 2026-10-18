namespace Sort
{
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
        /// <typeparam name="T">The type of elements in the array, must implement IComparable.</typeparam>
        /// <param name="array">The jagged array of rows to be sorted.</param>
        /// <param name="sort_directive">A 2D integer array specifying column indices and sort orders (1 for ascending, -1 for descending).</param>
        public static void FastSortArray<T>(this T[][] array, int[,] sort_directive)
            where T : IComparable<T>
        {
            // Guard clause: Exit early if the array is null or has 1 or fewer rows
            if (array == null || array.Length <= 1)
                return;

            // Extract the number of sorting rules from the 2D directive array
            int dirCount = sort_directive.GetLength(0);

            // Allocate a 1D struct array for directives to maximize CPU cache locality
            var directives = new SortDirective[dirCount];

            // Populate the 1D struct array from the 2D configuration matrix
            for (int i = 0; i < dirCount; i++)
            {
                directives[i] = new SortDirective
                {
                    Column = sort_directive[i, 0], // Target column index
                    Order = sort_directive[i, 1]   // Sort direction (1 = ASC, -1 = DESC)
                };
            }

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
        /// <param name="sort_directive">The sorting rules specifying column index and order.</param>
        public static void QuickSortArray<T>(this T[][] array, int[,] sort_directive)
            where T : IComparable<T>
        {
            // Exit if the array is empty or contains a single row
            if (array == null || array.Length <= 1)
                return;

            // Get total number of sorting rules
            int dirCount = sort_directive.GetLength(0);

            // Convert 2D rule matrix into a fast 1D struct array
            var directives = new SortDirective[dirCount];
            for (int i = 0; i < dirCount; i++)
            {
                directives[i] = new SortDirective
                {
                    Column = sort_directive[i, 0],
                    Order = sort_directive[i, 1]
                };
            }

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
        private static bool IsBeforeJagged<T>(T[] row, T[] pivot, SortDirective[] directives)
            where T : IComparable<T>
        {
            // Iterate through each sorting directive sequentially (primary, secondary, etc.)
            for (int i = 0; i < directives.Length; i++)
            {
                int col = directives[i].Column;
                int order = directives[i].Order;

                // Compare individual cell values at the target column
                int comparison = row[col].CompareTo(pivot[col]);

                // If values are not equal, determine precedence based on sort order
                if (comparison != 0)
                {
                    return order == 1 ? comparison < 0 : comparison > 0;
                }
            }
            // If all columns are identical, keep original relative order
            return true;
        }

        // Extremely fast row swapping via reference redirection (no cell copying)
        private static void SwapJaggedRows<T>(T[][] array, int i, int j)
        {
            T[] temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        #endregion

        #region Quick Sort for Traditional 2D Arrays (T[,])

        /// <summary>
        /// Sorts a traditional two-dimensional array based on sort directives using QuickSort.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The 2D array to sort.</param>
        /// <param name="sort_directive">The sorting directives.</param>
        public static void QuickSortArray<T>(this T[,] array, int[,] sort_directive)
            where T : IComparable<T>
        {
            // Guard clause for 2D array row count
            if (array == null || array.GetLength(0) <= 1)
                return;

            // Initiate recursive 2D QuickSort
            QuickSort2D(array, sort_directive, 0, array.GetLength(0) - 1);
        }

        // Recursive helper for 2D array QuickSort
        private static void QuickSort2D<T>(T[,] array, int[,] sort_directive, int low, int high)
            where T : IComparable<T>
        {
            if (low < high)
            {
                int pi = Partition2D(array, sort_directive, low, high);

                QuickSort2D(array, sort_directive, low, pi - 1);
                QuickSort2D(array, sort_directive, pi + 1, high);
            }
        }

        // Partitions a traditional 2D array by copying the pivot row into a temporary 1D buffer
        private static int Partition2D<T>(T[,] array, int[,] sort_directive, int low, int high)
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
                if (IsBefore2D(array, sort_directive, j, pivot))
                {
                    i++;
                    SwapRows2D(array, i, j); // Physically swaps cells column by column
                }
            }

            SwapRows2D(array, i + 1, high);
            return i + 1;
        }

        // Evaluates sorting precedence for 2D array elements
        private static bool IsBefore2D<T>(T[,] array, int[,] sort_directive, int row, T[] pivot)
            where T : IComparable<T>
        {
            for (int i = 0; i < sort_directive.GetLength(0); i++)
            {
                int column = sort_directive[i, 0];
                int order = sort_directive[i, 1];

                int comparison = array[row, column].CompareTo(pivot[column]);

                if (comparison != 0)
                {
                    if ((order == 1 && comparison > 0) || (order == -1 && comparison < 0))
                    {
                        return false;
                    }
                    else if ((order == -1 && comparison > 0) || (order == 1 && comparison < 0))
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        // Physically swaps all individual elements across columns for two rows in a 2D array
        private static void SwapRows2D<T>(T[,] array, int i, int j)
        {
            int cols = array.GetLength(1);
            for (int col = 0; col < cols; col++)
            {
                T temp = array[i, col];
                array[i, col] = array[j, col];
                array[j, col] = temp;
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
        /// <param name="sort_directive">The sorting directives.</param>
        public static void SelectionSortArray<T>(this T[,] array, int[,] sort_directive)
            where T : IComparable<T>
        {
            // Guard clause for null or single-row 2D arrays
            if (array == null || array.GetLength(0) <= 1)
                return;

            int arrayRows = array.GetLength(0);
            int arrayColumns = array.GetLength(1);
            int sortDirectiveColumns = sort_directive.GetLength(0);

            // Selection sort outer loop
            for (int i = 0; i < arrayRows - 1; i++)
            {
                int minIndex = i;

                // Inner loop to find the extreme element according to multi-column rules
                for (int j = i + 1; j < arrayRows; j++)
                {
                    bool shouldSwap = false;

                    for (int c = 0; c < sortDirectiveColumns; c++)
                    {
                        int columnToSort = sort_directive[c, 0];
                        int sortOrder = sort_directive[c, 1];

                        T valueI = array[minIndex, columnToSort];
                        T valueJ = array[j, columnToSort];

                        int comparisonResult = valueI.CompareTo(valueJ);

                        int compRule = sortOrder == -1 ? -comparisonResult : comparisonResult;
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
                        T temp = array[minIndex, d];
                        array[minIndex, d] = array[i, d];
                        array[i, d] = temp;
                    }
                }
            }
        }

        #endregion

        #region Shared Supporting Structures

        /// <summary>
        /// Represents a single sorting rule defining the target column index and sort order.
        /// </summary>
        private struct SortDirective
        {
            public int Column;
            public int Order;
        }

        /// <summary>
        /// A high-performance, zero-allocation struct-based comparer for jagged array rows.
        /// Enables JIT compiler devirtualization and inlining for maximum execution speed.
        /// </summary>
        private struct JaggedRowComparer<T> : IComparer<T[]>
            where T : IComparable<T>
        {
            private readonly SortDirective[] _directives;

            // Constructor initializing the sort rules inside the struct
            public JaggedRowComparer(SortDirective[] directives)
            {
                _directives = directives;
            }

            // Custom comparison logic executed millions of times during sorting
            public int Compare(T[] rowA, T[] rowB)
            {
                // Handle null reference edge cases
                if (rowA == rowB)
                    return 0;
                if (rowA == null)
                    return -1;
                if (rowB == null)
                    return 1;

                // Iterate through multi-column directives sequentially
                for (int i = 0; i < _directives.Length; i++)
                {
                    int col = _directives[i].Column;
                    int order = _directives[i].Order;

                    int comparison = rowA[col].CompareTo(rowB[col]);

                    // Return comparison outcome adjusted by sort direction
                    if (comparison != 0)
                    {
                        return order == 1 ? comparison : -comparison;
                    }
                }
                return 0;
            }
        }

        #endregion
    }
}