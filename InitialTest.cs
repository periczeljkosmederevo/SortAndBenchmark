namespace Sort
{
    public class InitialTest
    {

        public void Initial()
        {
            Console.WriteLine("Hello, World!");

            // Create and fill TABLE
            string[,] TABLE = new string[7, 3]
            {
                { "John", "Porter", "1234 Oak street, Pensacola, FL 32503" },
                { "John", "Porter", "1891 3rd Street North Westlake, OH 44145" },
                { "William", "Patterson", "4534 Virginia Street Dallas, GA 30132" },
                { "Marry", "Cortez", "7642 Fairview Avenue Milwaukee, WI 53204" },
                { "John", "Patterson", "1368 Street Road Morristown, NJ 07960" },
                { "Elizabet", "Cortez", "3698 Cedar Avenue Saratoga Springs, NY 12886" },
                { "Marry", "Mosley", "4575 11th Street Sacramento, CA 95820" }
            };

            //
            // Table SORT DIRECTIVE contains order of columns
            // by which array is going to be sorted and sort order
            // for each selected column
            //
            //  0 - do not sort column
            // -1 - sort in Descending order
            //  1 - sort in Ascending order
            //
            int[,] SORT_DIRECTIVE = new int[3, 2]
            {
                {0, 1},
                {1, -1},
                {2, 1}
            };

            //
            // Print initial TABLE
            //
            Console.WriteLine();
            Console.WriteLine("  INITIAL TABLE");
            Console.WriteLine(" --------------");
            Console.WriteLine();
            for (int n = 0; n < 7; n++)
            {
                Console.WriteLine("  {0,-10}{1,-10}{2,-10}", TABLE[n, 0], TABLE[n, 1], TABLE[n, 2]);
            }
            Console.WriteLine(" --------------");
            //

            //
            // Call sort method
            //
            TABLE.QuickSortArray(SORT_DIRECTIVE);


            //
            // Print sorted TABLE
            //
            Console.WriteLine();
            Console.WriteLine("  SORTED TABLE");
            Console.WriteLine(" --------------");
            Console.WriteLine();
            for (int n = 0; n < 7; n++)
            {
                Console.WriteLine("  {0,-10}{1,-10}{2,-10}", TABLE[n, 0], TABLE[n, 1], TABLE[n, 2]);
            }
            Console.WriteLine(" --------------");
            //
            Console.WriteLine();
            Console.CursorVisible = false;
            Console.Write(" Hit Any Key");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}
