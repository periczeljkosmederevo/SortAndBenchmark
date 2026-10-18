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

int rows = 10000;
int columns = 10;

// Liste za sakupljanje vremena iz svih 10 iteracija
List<double> times2D = new List<double>();
List<double> timesJagged = new List<double>();
List<double> timesFast = new List<double>();
List<double> timesLinq = new List<double>();

Stopwatch clock = new Stopwatch();

for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Pokretanje test iteracije {i + 1} od 10...");
    var randomList = RandomListGenerator.GenerateRandomList(rows, columns);

    // 1. Test za klasičan 2D niz (T[,]) - QuickSort
    string[,] randomTable = CopyListToArray(randomList);
    clock.Reset();
    clock.Start();
    randomTable.QuickSortArray(SORT_DIRECTIVE);
    clock.Stop();
    times2D.Add(clock.Elapsed.TotalSeconds);

    // 2. Test za Jagged Array Quick Sort (T[][])
    string[][] jaggedTable = CopyListToJaggedArray(randomList);
    clock.Reset();
    clock.Start();
    jaggedTable.QuickSortArray(SORT_DIRECTIVE);
    clock.Stop();
    timesJagged.Add(clock.Elapsed.TotalSeconds);

    // 3. Test za novu Fast Sort ekstenziju (.NET IntroSort + Zero Allocation Struct Comparer)
    string[][] jaggedTableFast = CopyListToJaggedArray(randomList);
    clock.Reset();
    clock.Start();
    jaggedTableFast.FastSortArray(SORT_DIRECTIVE);
    clock.Stop();
    timesFast.Add(clock.Elapsed.TotalSeconds);

    // 4. Test za LINQ (proširen na svih 10 kolona pravila)
    clock.Reset();
    clock.Start();
    var sortedTable = randomList
                    .OrderBy(row => row[0])
                    .ThenByDescending(row => row[1])
                    .ThenBy(row => row[2])
                    .ThenByDescending(row => row[3])
                    .ThenBy(row => row[4])
                    .ThenByDescending(row => row[5])
                    .ThenBy(row => row[6])
                    .ThenByDescending(row => row[7])
                    .ThenBy(row => row[8])
                    .ThenByDescending(row => row[9])
                    .ToList();
    clock.Stop();
    timesLinq.Add(clock.Elapsed.TotalSeconds);

    // Provera tačnosti (vrši se samo u prvoj iteraciji radi uštede vremena)
    if (i == 0)
    {
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

        Console.WriteLine(isEqual2D ? "-> 2D Array je identičan LINQ-u." : "-> 2D Array NIJE identičan LINQ-u.");
        Console.WriteLine(isEqualJagged ? "-> Jagged Quick Sort je identičan LINQ-u." : "-> Jagged Quick Sort NIJE identičan LINQ-u.");
        Console.WriteLine(isEqualFast ? "-> Fast Sort je identičan LINQ-u." : "-> Fast Sort NIJE identičan LINQ-u.");
        Console.WriteLine();
    }
}

// Metoda koja odbacuje minimum i maksimum, a računa prosek preostalih 8 vrednosti
double CalculateTrimmedAverage(List<double> times)
{
    times.Sort(); // Sortira vremena rastućim redosledom

    double sum = 0;
    // Preskačemo prvi (najmanji) i poslednji (najveći) element -> ostaje 8 merenja
    for (int i = 1; i < times.Count - 1; i++)
    {
        sum += times[i];
    }
    return sum / (times.Count - 2); // Deli sa 8
}

Console.WriteLine("\n==================================================");
Console.WriteLine(" KONAČNI REZULTATI (Prosek od 8 merenja, odbačeni min i max):");
Console.WriteLine("==================================================");
Console.WriteLine($"2D Quick Sort prosek:       {CalculateTrimmedAverage(times2D):F4} s");
Console.WriteLine($"Jagged Quick Sort prosek:   {CalculateTrimmedAverage(timesJagged):F4} s");
Console.WriteLine($"Fast Sort (.NET) prosek:    {CalculateTrimmedAverage(timesFast):F4} s");
Console.WriteLine($"LINQ prosek:                {CalculateTrimmedAverage(timesLinq):F4} s");
Console.WriteLine("==================================================");

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