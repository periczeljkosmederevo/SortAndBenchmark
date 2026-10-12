using System.Text;

namespace Sort
{
    public class RandomTableGenerator
    {
        private static Random random = new Random();

        public static string[,] GenerateRandomTable(int rows, int columns)
        {
            string[,] table = new string[rows, columns];
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < 1024; k++)
                    {
                        sb.Append(chars[random.Next(chars.Length)]);
                    }
                    table[i, j] = sb.ToString();
                }
            }
            return table;
        }

        public static string[,] DeepCopyTable(string[,] original)
        {
            int rows = original.GetLength(0);
            int columns = original.GetLength(1);
            string[,] copy = new string[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    copy[i, j] = original[i, j];
                }
            }
            return copy;
        }
    }
}

