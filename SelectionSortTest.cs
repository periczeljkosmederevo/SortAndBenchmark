using System.Diagnostics;

namespace Sort
{
    public class SelectionSortTest
    {
        public static void StartTest(int[,] SORT_DIRECTIVE, string[,] randomTable)
        {
            Stopwatch clock = new Stopwatch();

            clock.Start();
            clock.Stop();
            clock.Reset();

            clock.Start();

            randomTable.SelectionSortArray(SORT_DIRECTIVE);

            clock.Stop();

            Console.Write(" Selection sort algorithm : ");
            Console.WriteLine(clock.Elapsed.TotalSeconds);
            Console.WriteLine();
            Console.Write(" Hit Any Key");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}
