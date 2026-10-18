using System;
using System.Collections.Generic;
using System.Text;

namespace Sort
{
    public class RandomListGenerator
    {
        private static Random random = new Random();

        public static List<List<string>> GenerateRandomList(int rows, int columns)
        {
            var table = new List<List<string>>(rows);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Create a value pool of unique items for each column.
            // Decreasing the pool size (e.g., rows / 5 or rows / 10) increases duplicate values (ties),
            // forcing the algorithm to exercise deep multi-column sorting.
            int poolSize = Math.Max(10, rows / 5);

            var columnPools = new List<List<string>>(columns);
            for (int j = 0; j < columns; j++)
            {
                var pool = new List<string>(poolSize);
                for (int p = 0; p < poolSize; p++)
                {
                    StringBuilder sb = new StringBuilder(1024);
                    for (int k = 0; k < 1024; k++)
                    {
                        sb.Append(chars[random.Next(chars.Length)]);
                    }
                    pool.Add(sb.ToString());
                }
                columnPools.Add(pool);
            }

            // Generate rows by randomly selecting values from the pre-generated pool for each column
            for (int i = 0; i < rows; i++)
            {
                var row = new List<string>(columns);
                for (int j = 0; j < columns; j++)
                {
                    var pool = columnPools[j];
                    string randomValue = pool[random.Next(pool.Count)];
                    row.Add(randomValue);
                }
                table.Add(row);
            }

            return table;
        }

        public static List<List<string>> DeepCopyList(List<List<string>> original)
        {
            var copy = new List<List<string>>(original.Count);
            foreach (var row in original)
            {
                var newRow = new List<string>(row);
                copy.Add(newRow);
            }
            return copy;
        }
    }
}