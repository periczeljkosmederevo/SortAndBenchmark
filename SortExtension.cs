namespace Sort
{
    /// <summary>
    /// Provides extension methods for sorting two-dimensional arrays 
    /// using the QuickSort and Selection Sort algorithms.
    /// </summary>
    public static class SortExtensions
    {
        /// <summary>
        /// Sorts a two-dimensional array 
        /// based on sort directives using the QuickSort algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array. 
        /// Must be comparable.</typeparam>
        /// <param name="array">The two-dimensional array to be sorted.</param>
        /// <param name="sort_directive">A two-dimensional integer array 
        /// where the first column specifies the column index to sort by, 
        /// and the second column specifies the sort order 
        /// (1 for ascending, -1 for descending).</param>
        public static void QuickSortArray<T>(this T[,] array,
                                             int[,] sort_directive)
            where T : IComparable<T>
        {
            QuickSort(array, sort_directive, 0, array.GetLength(0) - 1);
        }

        #region QuickSortArray private
        /// <summary>
        /// Recursively sorts segments of the array 
        /// using the QuickSort algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array. 
        /// Must be comparable.</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="sort_directive">The sorting directives.</param>
        /// <param name="low">The starting index of the segment to sort.</param>
        /// <param name="high">The ending index of the segment to sort.</param>
        private static void QuickSort<T>(T[,] array,
                                         int[,] sort_directive,
                                         int low,
                                         int high)
            where T : IComparable<T>
        {
            if (low < high)
            {
                int pi = Partition(array, sort_directive, low, high);

                QuickSort(array, sort_directive, low, pi - 1);
                QuickSort(array, sort_directive, pi + 1, high);
            }
        }

        /// <summary>
        /// Partitions the array segment around 
        /// a pivot selected from the 'high' index, 
        /// according to the sorting directives.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array. 
        /// Must be comparable.</typeparam>
        /// <param name="array">The array to partition.</param>
        /// <param name="sort_directive">The sorting directives.</param>
        /// <param name="low">The starting index of the segment to partition.</param>
        /// <param name="high">The pivot index in the array.</param>
        /// <returns>The index of the pivot after partitioning.</returns>
        private static int Partition<T>(T[,] array,
                                        int[,] sort_directive,
                                        int low,
                                        int high)
            where T : IComparable<T>
        {
            T[] pivot = new T[array.GetLength(1)];

            int i = 0;

            for (i = 0; i < array.GetLength(1); i++)
            {
                pivot[i] = array[high, i];
            }

            i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (IsBefore(array, sort_directive, j, pivot))
                {
                    i++;
                    SwapRows(array, i, j);
                }
            }

            SwapRows(array, i + 1, high);

            return i + 1;
        }

        /// <summary>
        /// Compares a row with the pivot 
        /// and determines if it should precede 
        /// the pivot based on the sort directives.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array. 
        /// Must be comparable.</typeparam>
        /// <param name="array">The array containing the row.</param>
        /// <param name="sort_directive">The sorting directives.</param>
        /// <param name="row">The index of the row to compare.</param>
        /// <param name="pivot">The pivot row values.</param>
        /// <returns>True if the row should precede the pivot; 
        /// otherwise, false.</returns>
        private static bool IsBefore<T>(T[,] array,
                                        int[,] sort_directive,
                                        int row,
                                        T[] pivot)
            where T : IComparable<T>
        {
            for (int i = 0; i < sort_directive.GetLength(0); i++)
            {
                int column = sort_directive[i, 0];
                int order = sort_directive[i, 1];

                int comparison = array[row, column].CompareTo(pivot[column]);

                if (comparison != 0)
                {
                    if ((order == 1 && comparison > 0) ||
                        (order == -1 && comparison < 0))
                    {
                        return false;
                    }
                    else if ((order == -1 && comparison > 0) ||
                             (order == 1 && comparison < 0))
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Swaps two rows in the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array containing the rows to swap.</param>
        /// <param name="i">The index of the first row.</param>
        /// <param name="j">The index of the second row.</param>
        private static void SwapRows<T>(T[,] array, int i, int j)
        {
            for (int col = 0; col < array.GetLength(1); col++)
            {
                T temp = array[i, col];
                array[i, col] = array[j, col];
                array[j, col] = temp;
            }
        }
        #endregion

        /// <summary>
        /// Sorts a two-dimensional array 
        /// based on sort directives using the Selection Sort algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array. 
        /// Must be comparable.</typeparam>
        /// <param name="array">The two-dimensional array to be sorted.</param>
        /// <param name="sort_directive">A two-dimensional integer array 
        /// where the first column specifies the column index to sort by, 
        /// and the second column specifies the sort order 
        /// (1 for ascending, -1 for descending).</param>
        public static void SelectionSortArray<T>(this T[,] array,
                                                 int[,] sort_directive)
            where T : IComparable<T>
        {
            int arrayRows = array.GetLength(0);
            int arrayColumns = array.Length / arrayRows;
            int sortDirectiveColumns = sort_directive.GetLength(0);

            for (int i = 0; i < arrayRows - 1; i++)
            {
                int minIndex = i;

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

                        if ((sortOrder == -1 && comparisonResult < 0) ||
                            (sortOrder == 1 && comparisonResult > 0))
                        {
                            shouldSwap = true;
                            break;
                        }
                        else if ((sortOrder == -1 && comparisonResult > 0) ||
                                 (sortOrder == 1 && comparisonResult < 0))
                        {
                            break;
                        }
                    }

                    if (shouldSwap)
                    {
                        minIndex = j;
                    }
                }

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

    }
}
