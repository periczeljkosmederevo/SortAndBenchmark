using System.Text;

namespace Sort
{
    public class RandomListGenerator
    {
        private static Random random = new Random();

        public static List<List<string>> GenerateRandomList(int rows, int columns)
        {
            var table = new List<List<string>>();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (int i = 0; i < rows; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < columns; j++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < 1024; k++)
                    {
                        sb.Append(chars[random.Next(chars.Length)]);
                    }
                    row.Add(sb.ToString());
                }
                table.Add(row);
            }
            return table;
        }

        public static List<List<string>> DeepCopyList(List<List<string>> original)
        {
            var copy = new List<List<string>>();
            foreach (var row in original)
            {
                var newRow = new List<string>(row);
                copy.Add(newRow);
            }
            return copy;
        }
    }
}
