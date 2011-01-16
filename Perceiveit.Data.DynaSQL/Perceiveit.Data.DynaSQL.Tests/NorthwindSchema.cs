using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.DynaSql.Tests
{
    /// <summary>
    /// Wrapper for the database schema of the northwind database
    /// </summary>
    /// <remarks>Comment and uncomment the sections for each database implementation</remarks>
    public static class Nw
    {

        /* ----------  MS SQL Server Northwind Schema -------------- */

        public const string DbConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True;Pooling=False";
        public const string DbProvider = "System.Data.SqlClient";

        public static class Customers
        {
            public const string Table = "Customers";
            public const string CustomerID = "CustomerID";
            public const string CompanyName = "CompanyName";
            public const string Region = "Region";
        }

        public static class Categories
        {
            public const string Table = "Categories";
            public const string CategoryID = "CategoryID";
            public const string CategoryName = "CategoryName";
            public const string Description = "Description";
            public const string Picture = "Picture";
        }

        public static class Orders
        {
            public const string Table = "Orders";
            public const string OrderID = "OrderID";
            public const string CustomerID = "CustomerID";
            public const string EmployeeID = "EmployeeID";
            public const string OrderDate = "OrderDate";
        }

        public static class OrderDetails
        {
            public const string Table = "Order Details";
            public const string ID = "ID";
            public const string OrderID = "OrderID";
            public const string ProductID = "ProductID";
            public const string UnitPrice = "UnitPrice";
            public const string Quantity = "Quantity";
            public const string Discount = "Discount";
        }

        public static class Products
        {
            public const string Table = "Products";
            public const string ProductID = "ProductID";
            public const string ProductName = "ProductName";
            public const string SupplierID = "SupplierID";
            public const string CategoryID = "CategoryID";

        }

        public static class Employees
        {
            public const string Table = "Employees";
            public const string EmployeeID = "EmployeeID";

        }

        /* -------------------------- End MS SQL Server Schema ------------------------- */


        /* ----------  MySQL Northwind Schema -------------- 


        public const string DbConnection = "server=172.16.56.1;User Id=testaccount;Password=test;Persist Security Info=True;database=northwind";
        public const string DbProvider = "MySql.Data.MySqlClient";
         
        public static class Customers
        {
            public const string Table = "Customers";
            public const string CustomerID = "CustomerID";
            public const string CompanyName = "CompanyName";
            public const string Region = "Region";
        }

        public static class Categories
        {
            public const string Table = "Categories";
            public const string CategoryID = "CategoryID";
            public const string CategoryName = "CategoryName";
            public const string Description = "Description";
            public const string Picture = "Picture";
        }

        public static class Orders
        {
            public const string Table = "Orders";
            public const string OrderID = "OrderID";
            public const string CustomerID = "CustomerID";
            public const string EmployeeID = "EmployeeID";
            public const string OrderDate = "OrderDate";
        }

        public static class OrderDetails
        {
            //only change from MSSql in current schema
            public const string Table = "Order_Details";
            public const string ID = "ID";
            public const string OrderID = "OrderID";
            public const string ProductID = "ProductID";
            public const string UnitPrice = "UnitPrice";
            public const string Quantity = "Quantity";
            public const string Discount = "Discount";
        }

        public static class Products
        {
            public const string Table = "Products";
            public const string ProductID = "ProductID";
            public const string ProductName = "ProductName";
            public const string SupplierID = "SupplierID";
            public const string CategoryID = "CategoryID";

        }
        
        public static class Employees
        {
            public const string Table = "Employees";
            public const string EmployeeID = "EmployeeID";

        }

        /* -------------------------- End MySQL Schema ------------------------- */


        /* ----------  Access 2007 Northwind Schema -------------- 
        //NOTE: x86 compilation only and script execution is not supported
        public const string DbConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=""C:\Users\Richard Hewitson\Documents\Northwind07.accdb"";Persist Security Info=False;";
        public const string DbProvider = "System.Data.OleDb";

        public static class Customers
        {
            public const string Table = "Customers";
            public const string CustomerID = "ID";
            public const string CompanyName = "Company";
            public const string Region = "Country_Region"; //changed from a back slash in the database to check support
        }

        public static class Categories
        {
            public const string Table = "Categories";
            public const string CategoryID = "CategoryID";
            public const string CategoryName = "Name";
            public const string Description = "Description";
            public const string Picture = "Picture";
        }

        public static class Orders
        {
            public const string Table = "Orders";
            public const string OrderID = "Order ID";
            public const string CustomerID = "Customer ID";
            public const string EmployeeID = "Employee ID";
            public const string OrderDate = "Order Date";
        }

        public static class OrderDetails
        {
            public const string Table = "Order Details";
            public const string ID = "ID";
            public const string OrderID = "Order ID";
            public const string ProductID = "Product ID";
            public const string UnitPrice = "Unit Price";
            public const string Quantity = "Quantity";
            public const string Discount = "Discount";
        }

        public static class Products
        {
            public const string Table = "Products";
            public const string ProductID = "ID";
            public const string ProductName = "Product Name";
            public const string SupplierID = "Supplier IDs";
            public const string CategoryID = "Category ID";

        }
       
        public static class Employees
        {
            public const string Table = "Employees";
            public const string EmployeeID = "EmployeeID";

        }
        
       /* -------------------------- End Access 2007 Schema ------------------------- */

        /* ----------  SQLite Northwind Schema -------------- 


        public const string DbConnection = @"data source=C:\Development\Perceiveit.Data.DynaSQL\Northwind_sqlite\Northwind.sl3";
       public const string DbProvider = "System.Data.SQLite";
         
       public static class Customers
       {
           public const string Table = "Customers";
           public const string CustomerID = "CustomerID";
           public const string CompanyName = "CompanyName";
           public const string Region = "Region";
       }

       public static class Categories
       {
           public const string Table = "Categories";
           public const string CategoryID = "CategoryID";
           public const string CategoryName = "CategoryName";
           public const string Description = "Description";
           public const string Picture = "Picture";
       }

       public static class Orders
       {
           public const string Table = "Orders";
           public const string OrderID = "OrderID";
           public const string CustomerID = "CustomerID";
           public const string EmployeeID = "EmployeeID";
           public const string OrderDate = "OrderDate";
       }

       public static class OrderDetails
       {
           public const string Table = "Order Details";
           public const string ID = "ID";
           public const string OrderID = "OrderID";
           public const string ProductID = "ProductID";
           public const string UnitPrice = "UnitPrice";
           public const string Quantity = "Quantity";
           public const string Discount = "Discount";
       }

       public static class Products
       {
           public const string Table = "Products";
           public const string ProductID = "ProductID";
           public const string ProductName = "ProductName";
           public const string SupplierID = "SupplierID";
           public const string CategoryID = "CategoryID";

       }
       
       public static class Employees
       {
            public const string Table = "Employees";
            public const string EmployeeID = "EmployeeID";

       }

       /* -------------------------- End SQLite Schema ------------------------- */
    }
}
