using Sort;
using System.Diagnostics;

new InitialTest().Initial();

int[,] SORT_DIRECTIVE;
SORT_DIRECTIVE = new int[5, 2]
{
    {0, 1},
    {1, -1},
    {2, 1},
    {3, -1},
    {4, 1}
};

int rows = 1000000;
int columns = 5;
string[,] randomTable = new string[rows,columns];
//string[,] randomTable;


// Initialization of the Stopwatch outside the loop
Stopwatch clock = new Stopwatch();


for (int i = 0; i < 1; i++)
{
    var randomList = RandomListGenerator.GenerateRandomList(rows, columns);
    randomTable = CopyListToArray(randomList);

    clock.Reset();
    clock.Start();

    // Assuming you have a QuickSort implementation for arrays
    randomTable.QuickSortArray(SORT_DIRECTIVE);

    clock.Stop();

    Console.Write("Quick sort algorithm: ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();

    clock.Reset();
    clock.Start();

    var sortedTable = randomList
                    .OrderBy(row => row[0])
                    .ThenByDescending(row => row[1])
                    .ThenBy(row => row[2])
                    .ThenByDescending(row => row[3])
                    .ThenBy(row => row[4])
                    .ToList();

    clock.Stop();

    Console.Write("LINQ: ");
    Console.WriteLine(clock.Elapsed.TotalSeconds);
    Console.WriteLine();



    // Check if the two sorted collections have the same result
    bool isEqual = true;
    for (int j = 0; j < sortedTable.Count; j++)
    {
        for (int k = 0; k < columns; k++)
        {
            if (sortedTable[j][k] != randomTable[j, k])
            {
                isEqual = false;
                break;
            }
        }
        if (!isEqual)
        {
            Console.WriteLine("Arrays are not equal.");
            break;
        }
    }

    if (isEqual)
    {
        Console.WriteLine("Arrays are equal.");
    }
}




//if (rows <= 10000)
//{
//    new SelectionSortTest().StartTest(SORT_DIRECTIVE, randomTable1);
//}

//for (int i = 0; i < 20; i++)
//{
//    randomTable2 = RandomTableGenerator.GenerateRandomTable(rows, columns);
//    new QuickSortTest().StartTest(SORT_DIRECTIVE, randomTable2);
//}

Console.WriteLine(" End");
Console.WriteLine("hit any key");
Console.ReadKey();

static string[,] CopyListToArray(List<List<string>> list)
{
    int rows = list.Count;
    int columns = list[0].Count; // Assuming all inner lists have the same length

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