using Sort;
using System.Diagnostics;

new InitialTest().Initial();

int[,] SORT_DIRECTIVE = new int[10, 2]
{
    {0, 1},
    {1, -1},
    {2, 1},
    {3, -1},
    {4, 1},
    {5, 1},
    {6, -1},
    {7, 1},
    {8, -1},
    {9, 1}
};

int rows = 1000000;
int columns = 10;
Stopwatch clock = new Stopwatch();

for (int i = 0; i < 1; i++)
{
    var randomList = RandomListGenerator.GenerateRandomList(rows, columns);

    // 1. Test za klasičan 2D niz (T[,]) - QuickSort
    string[,] randomTable = CopyListToArray(randomList);
    clock.Reset();
    clock.Start();
    randomTable.QuickSortArray(SORT_DIRECTIVE);
    clock.Stop();

    Console.Write("2D Quick Sort algorithm: ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();

    // 2. Test za Jagged Array Quick Sort (T[][])
    string[][] jaggedTable = CopyListToJaggedArray(randomList);
    clock.Reset();
    clock.Start();
    jaggedTable.QuickSortArray(SORT_DIRECTIVE);
    clock.Stop();

    Console.Write("Jagged Array Quick Sort algorithm: ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();

    // 3. Test za novu Fast Sort ekstenziju (.NET IntroSort + Zero Allocation Struct Comparer)
    string[][] jaggedTableFast = CopyListToJaggedArray(randomList);
    clock.Reset();
    clock.Start();
    jaggedTableFast.FastSortArray(SORT_DIRECTIVE);
    clock.Stop();

    Console.Write("Fast Sort algorithm (.NET IntroSort): ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();

    // 4. Test za LINQ (proširen na svih 10 kolona pravila)
    clock.Reset();
    clock.Start();
    var sortedTable = randomList
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
    clock.Stop();

    Console.Write("LINQ: ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();

    // Provera tačnosti (upoređivanje rezultata sa LINQ-om)
    bool isEqual2D = true;
    bool isEqualJagged = true;
    bool isEqualFast = true;

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

    Console.WriteLine(isEqual2D ? "2D Array is equal to LINQ." : "2D Array is NOT equal to LINQ.");
    Console.WriteLine(isEqualJagged ? "Jagged Array Quick Sort is equal to LINQ." : "Jagged Array Quick Sort is NOT equal to LINQ.");
    Console.WriteLine(isEqualFast ? "Fast Sort (.NET IntroSort) is equal to LINQ." : "Fast Sort is NOT equal to LINQ.");
}

Console.WriteLine("End");
Console.WriteLine("Hit any key");
Console.ReadKey();

static string[,] CopyListToArray(List<List<string>> list)
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

static string[][] CopyListToJaggedArray(List<List<string>> list)
{
    int rows = list.Count;
    string[][] array = new string[rows][];

    for (int i = 0; i < rows; i++)
    {
        array[i] = list[i].ToArray();
    }

    return array;
}