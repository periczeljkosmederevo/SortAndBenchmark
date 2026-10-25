namespace SortAndBenchmark
{
    /// <summary>
    /// Provides robust utility methods for generating synthetic test datasets with controlled 
    /// duplicate densities (ties) and performing deep copies for benchmarking purposes.
    /// </summary>
    public class RandomListGenerator
    {
        // Thread-safe / static random instance for pseudo-random data generation.
        private static readonly Random _random = new();

        /// <summary>
        /// Generates a randomized two-dimensional list of strings structured into rows and columns,
        /// specifically engineered for rigorous multi-column sorting benchmarks with high tie frequency.
        /// </summary>
        /// <param name="rows">The number of rows (dataset scale) to generate.</param>
        /// <param name="columns">The number of columns per row.</param>
        /// <returns>A fully populated List of Lists representing the synthetic table dataset.</returns>
        public static List<List<string>> GenerateRandomList(int rows, int columns)
        {
            var table = new List<List<string>>(rows);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Create a value pool of unique items for each column.
            // Decreasing the pool size (e.g., rows / 5) increases duplicate values (ties),
            // forcing the sorting algorithm to exercise deep multi-column fallback logic (ThenBy).
            // Each generated string has a fixed length of 1024 characters to simulate heavy payload comparisons.
            int poolSize = Math.Max(10, rows / 5);

            var columnPools = new List<List<string>>(columns);
            for (int j = 0; j < columns; j++)
            {
                var pool = new List<string>(poolSize);
                for (int p = 0; p < poolSize; p++)
                {
                    StringBuilder sb = new(1024);
                    for (int k = 0; k < 1024; k++)
                    {
                        sb.Append(chars[_random.Next(chars.Length)]);
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
                    string randomValue = pool[_random.Next(pool.Count)];
                    row.Add(randomValue);
                }
                table.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Creates a complete, independent deep copy of a nested list structure,
        /// ensuring that sorting or modifications to the copy do not affect the original source data.
        /// </summary>
        /// <param name="original">The source List of Lists to copy.</param>
        /// <returns>A new, structurally identical List of Lists with newly allocated inner lists.</returns>
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