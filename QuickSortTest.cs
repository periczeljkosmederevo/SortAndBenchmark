using System;
using System.Diagnostics;

namespace Sort
{
    public class QuickSortTest
    {
        public static void StartTest(SortDirective[] SORT_DIRECTIVE, string[,] randomTable)
        {
            Stopwatch clock = new Stopwatch();

            clock.Start();
            clock.Stop();
            clock.Reset();

            clock.Start();

            // Calling QuickSortArray with the new SortDirective array model
            randomTable.QuickSortArray(SORT_DIRECTIVE);

            clock.Stop();

            Console.Write(" Quick sort algorithm : ");
            Console.WriteLine(clock.Elapsed.TotalSeconds);
            Console.WriteLine();
            Console.Write(" Hit Any Key");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}