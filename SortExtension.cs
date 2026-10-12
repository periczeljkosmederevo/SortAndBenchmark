using System;
using System.Collections.Generic;

namespace Sort
{
    /// <summary>
    /// Provides high-performance extension methods for sorting multi-dimensional (2D) 
    /// and jagged arrays using advanced sorting algorithms and custom multi-column directives.
    /// </summary>
    public static class SortExtensions
    {
        #region Fast Sort (Jagged Arrays - Beats LINQ)

        /// <summary>
        /// Sorts a jagged array using the .NET IntroSort algorithm paired with a zero-allocation 
        /// struct comparer. This method provides maximum performance, outperforming LINQ.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array, must implement IComparable.</typeparam>
        /// <param name="array">The jagged array of rows to be sorted.</param>
        /// <param name="sort_directive">A 2D integer array specifying column indices and sort orders (1 for ascending, -1 for descending).</param>
        public static void FastSortArray<T>(this T[][] array, int[,] sort_directive)
            where T : IComparable<T>
        {
            if (array == null || array.Length <= 1)
                return;

            // Convert 2D directive array into a 1D struct array for optimal CPU cache locality
            int dirCount = sort_directive.GetLength(0);
            var directives = new SortDirective[dirCount];
            for (int i = 0; i < dirCount; i++)
            {
                directives[i] = new SortDirective
                {
                    Column = sort_directive[i, 0],
                    Order = sort_directive[i, 1]
                };
            }

            // Invoke optimized .NET Array.Sort with a custom struct comparer (Zero Heap Allocations)
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
            if (array == null || array.Length <= 1)
                return;

            int dirCount = sort_directive.GetLength(0);
            var directives = new SortDirective[dirCount];
            for (int i = 0; i < dirCount; i++)
            {
                directives[i] = new SortDirective
                {
                    Column = sort_directive[i, 0],
                    Order = sort_directive[i, 1]
                };
            }

            QuickSortJagged(array, directives, 0, array.Length - 1);
        }

        private static void QuickSortJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            if (low < high)
            {
                int pi = PartitionJagged(array, directives, low, high);

                QuickSortJagged(array, directives, low, pi - 1);
                QuickSortJagged(array, directives, pi + 1, high);
            }
        }

        private static int PartitionJagged<T>(T[][] array, SortDirective[] directives, int low, int high)
            where T : IComparable<T>
        {
            T[] pivot = array[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (IsBeforeJagged(array[j], pivot, directives))
                {
                    i++;
                    SwapJaggedRows(array, i, j);
                }
            }

            SwapJaggedRows(array, i + 1, high);
            return i + 1;
        }

        private static bool IsBeforeJagged<T>(T[] row, T[] pivot, SortDirective[] directives)
            where T : IComparable<T>
        {
            for (int i = 0; i < directives.Length; i++)
            {
                int col = directives[i].Column;
                int order = directives[i].Order;

                int comparison = row[col].CompareTo(pivot[col]);

                if (comparison != 0)
                {
                    return order == 1 ? comparison < 0 : comparison > 0;
                }
            }
            return true;
        }

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
            if (array == null || array.GetLength(0) <= 1)
                return;

            QuickSort2D(array, sort_directive, 0, array.GetLength(0) - 1);
        }

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

        private static int Partition2D<T>(T[,] array, int[,] sort_directive, int low, int high)
            where T : IComparable<T>
        {
            int cols = array.GetLength(1);
            T[] pivot = new T[cols];

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
                    SwapRows2D(array, i, j);
                }
            }

            SwapRows2D(array, i + 1, high);
            return i + 1;
        }

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
            if (array == null || array.GetLength(0) <= 1)
                return;

            int arrayRows = array.GetLength(0);
            int arrayColumns = array.GetLength(1);
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

            public JaggedRowComparer(SortDirective[] directives)
            {
                _directives = directives;
            }

            public int Compare(T[] rowA, T[] rowB)
            {
                if (rowA == rowB)
                    return 0;
                if (rowA == null)
                    return -1;
                if (rowB == null)
                    return 1;

                for (int i = 0; i < _directives.Length; i++)
                {
                    int col = _directives[i].Column;
                    int order = _directives[i].Order;

                    int comparison = rowA[col].CompareTo(rowB[col]);

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