using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.DynaSql.Tests
{
    class Program
    {

        static void Main(string[] args)
        {
            bool contine = true;

            while (contine)
            {
                Console.WriteLine();
                Console.WriteLine("Beginning tests");
                Console.WriteLine();

                try
                {
                    DynaSQLTests dsql = new DynaSQLTests();
                    dsql.SetUpConnectionDb();
                    dsql.Northwind_01_OrderCountTest();
                    dsql.Northwind_02_OrderToOrderDetails();
                    dsql.Northwind_03_ExistsTest();
                    dsql.Northwind_04_ArrayTestWithDelegate();
                    dsql.Northwind_05_UpdateTests();
                    dsql.Northwind_06_InsertTest();
                    dsql.Northwind_07_InsertListAndReturnIDs();
                    dsql.Northwind_08_WhereSubSelect();
                    dsql.Northwind_09_BigSubSelect();
                    dsql.Northwind_10_MultipleResults();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("*********************  Execute failed  *********************");
                    while (ex != null)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine();
                        ex = ex.InnerException;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Competed tests. Press any key to run again, or 'q' to quit");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    contine = false;
            }
        }
    }
}
