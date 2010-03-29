using System;
using System.Collections.Generic;
using System.Text;
using Perceiveit.Data;
using Perceiveit.Data.Query;
using System.Data.Common;
using System.Data;

namespace Perceiveit.Data.DynaSql.Tests
{
    /// <summary>
    /// Tests the generation of SQL Statements against a specific database provider.
    /// All queries use the Northwind database schema - defined in the NorthwindSchema.cs <see cref="Perceiveit.Data.DynaSql.Tests.Nw"/> file.
    /// </summary>
    /// <remarks>Each database schema I could find for Northwind has slightly different names
    /// so the Nw class has comment sections to use these specific names</remarks>
    [NUnit.Framework.TestFixture()]
    public class DynaSQLTests
    {

        [NUnit.Framework.TestFixtureSetUp()]
        public void SetUpConnectionDb()
        {
            //set up is called once for the test suite execution. Here we 
            //output the connection properties we are using just for identification purposes
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            Console.WriteLine("DataBase Properties: " + db.GetProperties().ToString());
        }

        public List<MyClass> SimpleQuery()
        {
            DBDatabase db = DBDatabase.Create("ConnectionName");

            DBQuery sel = DBQuery.SelectAll()
                                 .From("MyTable")
                                 .WhereField("Name", Compare.Like, DBConst.String("filter%"));

            List<MyClass> list = new List<MyClass>();

            int count = (int)db.ExecuteRead(delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    MyClass c = new MyClass();
                    c.ID = Convert.ToInt32(reader["ID"]);
                    c.Name = Convert.ToString(reader["Name"]);
                    list.Add(c);
                }
                return list.Count;
            });

            return list;
        }

        /// <summary>
        /// Extracts a count of all the orders for all customers after a specified date
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_01_OrderCountTest()
        {
            //initialize
            StringBuilder names = new StringBuilder();
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            //date limit
            DateTime ordersAfter = new DateTime(1990,01,01);
            
            //Create the select query
            DBSelectQuery sel = DBQuery.Select()
                                             .Field(Nw.Orders.CustomerID)
                                             .Count(Nw.Orders.OrderID) //short hand for .Aggregate(AggregateFunction.Count)
                                                                       //also supports Sum, Min, Max
                                        .From(Nw.Orders.Table)
                                        .GroupBy(Nw.Orders.CustomerID)
                                        .OrderBy(Nw.Orders.CustomerID)
                                        .WhereField(Nw.Orders.OrderDate, 
                                                        Compare.GreaterThan, DBConst.DateTime(ordersAfter));
            //write the SQL to the console
            OutputCommand(db, sel);

            //execute the read and populate the string builder
            int count = (int)db.ExecuteRead(sel, delegate(DbDataReader reader)
            {
                int total = 0;

                while (reader.Read()){
                    names.AppendFormat("{0} = {1}, ", reader[0], reader[1]);
                    total++;
                }

                return total;
            });


            //finally stop the stopwatch and output the results
            sw.Stop();
            
            Console.WriteLine("Total read time : " + sw.Elapsed.ToString());
            Console.WriteLine("Count for Customers = {0}", count);
            Console.WriteLine("Customers : " + names.ToString());
            Console.WriteLine();
        }

        /// <summary>
        /// Extracts all the orders with the number of Order Detail items for a
        /// specific customer - specified via a parameter
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_02_OrderToOrderDetails()
        {
            //initialize
            StringBuilder names = new StringBuilder();
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            //A static parameter with the specified value
            DBParam cid = DBParam.ParamWithValue("BERGS");
            

            //Create the select query
            DBQuery sel = DBQuery.Select().Field(Nw.Orders.CustomerID)
                                          .Field("O", Nw.Orders.OrderID)
                                          .Sum("OD",Nw.OrderDetails.Quantity).As("itemCount")
                                   .From(Nw.Orders.Table).As("O")
                                        .InnerJoin(Nw.OrderDetails.Table).As("OD")
                                              .On("O", Nw.Orders.OrderID, Compare.Equals, "OD", Nw.OrderDetails.OrderID)
                                   .WhereField(Nw.Orders.CustomerID, Compare.Equals, cid)//parameter
                                   .GroupBy(Nw.Orders.CustomerID).And("O", Nw.Orders.OrderID)
                                   .OrderBy(Nw.Orders.CustomerID);
            
            //write the SQL to the console
            OutputCommand(db, sel);

            //Execute the read and populate the string builder
            int count = (int)db.ExecuteRead(sel, delegate(DbDataReader reader)
            {
                int total = 0;
                while (reader.Read())
                {
                    names.AppendFormat("{0}({1}) = {2}, ", reader[0], reader[1], reader[2]);
                    total++;
                }
                return total;
            });


            //finally stop the stopwatch and output the results
            sw.Stop();
            Console.WriteLine("Total read time : " + sw.Elapsed.ToString());
            Console.WriteLine("Count for Customer Orders = {0}", count);
            Console.Write("Item count for customer '" + cid.Value.ToString() + "' orders :");
            Console.WriteLine(names);
        }

        /// <summary>
        /// Checks the wherein construct against an array of integers
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_03_ExistsTest()
        {
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);

            DBSelectQuery select = DBQuery.SelectCount()
                                          .From(Nw.Categories.Table)
                                          .WhereIn(Nw.Categories.CategoryID, 1, 2, 3, 4);

            //write the SQL to the console
            OutputCommand(db, select);

            int count = Convert.ToInt32(db.ExecuteScalar(select));
            NUnit.Framework.Assert.AreEqual(4, count);

            //output the results of the query
            Console.WriteLine("There are '{0}' rows with ids of '1,2,3 or 4'", count);

        }


        /// <summary>
        /// Executes the sames statement multiple times based on the array
        /// of integers for product ids.
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_04_ArrayTestWithDelegate()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            List<int> ids = new List<int>();
            Dictionary<int, int> counts = new Dictionary<int, int>();
            Dictionary<int, double> sums = new Dictionary<int, double>();


            //the orders list to inspect.
            List<int> productids = new List<int>(new int[] { 10, 11, 12 });

            int index = 0;//have to set the value because VS thinks it is unassigned.

            

            //create a parameter whose value is sourced via an anonymous delegate
            DBParam pid = DBParam.ParamWithDelegate("productid", System.Data.DbType.String, 
                delegate 
                {
                    //Console.WriteLine("Get order id from the array for index '" + index + "' = " + orderids[index]);
                    
                    //the index is modified in the execution loop below - it is evaluated when the
                    //method is called, not here where it is created.
                    return productids[index].ToString();
                });

            
            //create the select query
            //using the overloaded operators for the sum
            //fields, constants, aggregates, boolean ops all inherit from DBCalculableClause
            //which implements the operator overloads so they can be joined in a single statement
            DBSelectQuery select = DBSelectQuery
                .SelectCount().As("count")
                    .Sum(DBField.Field(Nw.OrderDetails.UnitPrice)
                                        * DBField.Field(Nw.OrderDetails.Quantity))
                                 .As("sum")
                    .And(Nw.OrderDetails.ProductID)
                .From(Nw.OrderDetails.Table)
                .WhereFieldEquals(Nw.OrderDetails.ProductID, pid) //using the delegate parameter
                .GroupBy(Nw.OrderDetails.ProductID);


            //write the SQL to the console
            //This will also call the parameter delegate method when creating the statement
            OutputCommand(db, select);


            //now if we loop through the parameter 'productids' it will use the
            //value of index (in the anonymous delegate) to get the orderid for the query
            for (index = 0; index < productids.Count; index++)
            {
                db.ExecuteRead(select, delegate(DbDataReader reader)
                {
                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0]);
                        double sum = Convert.ToDouble(reader[1]);
                        int id = Convert.ToInt32(reader[2]);
                        //populate the collections so we can enumerate over them later
                        counts.Add(id, count);
                        sums.Add(id, sum);
                        ids.Add(id);
                    }
                    return null;
                });
            }
            
            //finally output the results
            Console.WriteLine("Results from Collection Test");
            Console.WriteLine("ids .count = " + ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                Console.WriteLine("{0}: productID={1}, count={2}, sum={3}", i, ids[i], counts[ids[i]], sums[ids[i]]);
            }
                
        }

        /// <summary>
        /// Updates the name and description of a category and checks that the name was updated.
        /// Finally resetting it to its previous value
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_05_UpdateTests()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            string origname = string.Empty;
            string origdesc = string.Empty;
            string newname = string.Empty;
            string newdesc = string.Empty;
            int updated = 0;

            //The id of the category to modify
            DBConst catid = DBConst.Const(1);

            //select query
            DBSelectQuery sel = DBQuery.SelectFields(Nw.Categories.CategoryName, Nw.Categories.Description)
                                       .From(Nw.Categories.Table)
                                       .WhereFieldEquals(Nw.Categories.CategoryID, catid);


            //update query with parameters
            DBParam cname = DBParam.ParamWithValue("test 2");
            DBParam cdesc = DBParam.ParamWithValue("this is the new description");

            DBUpdateQuery update = DBUpdateQuery.Update(Nw.Categories.Table)
                                        .Set(Nw.Categories.CategoryName, cname)
                                        .AndSet(Nw.Categories.Description, cdesc)
                                    .WhereField(Nw.Categories.CategoryID, Compare.Equals, DBConst.Const(1));

            //write the SQL to the console
            OutputCommand(db, sel);
            OutputCommand(db, update);

            //now execute under a transaction
            //the transaction will not be of the standard connection type
            //but a wrapped transaction that will dispose of its connection when it is disposed
            DbTransaction transaction = db.BeginTransaction();


            try
            {
                //read the original values
                db.ExecuteRead(transaction, sel, delegate(DbDataReader reader)
                {
                    if (reader.Read())
                    {
                        origname = reader.GetString(0);
                        origdesc = reader.GetString(1);
                        return 1;
                    }
                    else
                        return 0;
                });

                //update the row with the new values
                updated = db.ExecuteNonQuery(transaction, update);
                Console.WriteLine("Updated '" + updated.ToString() + "' rows with the new values");

                //re-read the values back from the database
                db.ExecuteRead(transaction, sel, delegate(DbDataReader reader)
                {
                    if (reader.Read())
                    {
                        newname = reader.GetString(0);
                        newdesc = reader.GetString(1);
                        return 1;
                    }
                    else
                        return 0;
                });

                //check against the specifed parameter values above
                NUnit.Framework.Assert.AreEqual(newname, "test 2");
                NUnit.Framework.Assert.AreEqual(newdesc, "this is the new description");
                Console.WriteLine("Values have been changed to the new values of '" + newname + "' and '" + newdesc + "'");


                //throw an exception and confirm that the action was rolled back in the transaction
                //or comment out to fully execute the resetting of the values
                throw new ArgumentException("Intentional exception");


                //restore the previous values by setting the parameter values
                cname.Value = origname;
                cdesc.Value = origdesc;
                updated = db.ExecuteNonQuery(transaction, update);
                Console.WriteLine("Updated '" + updated.ToString() + "' rows with the original values");


                //commit the transaction as successfully executed
                transaction.Commit();
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "Intentional exception")
                    Console.WriteLine("Caught and sank the intentional exception");//this should be sunk
                else
                    throw;
            }
            finally
            {
                //transaction will be disposed (and rolled back if it has not been committed)
                transaction.Dispose();
            }

            //execute a read outside of the transaction to confirm successfull execution
            //or to make sure that the transaction has been rolled back
            db.ExecuteRead(sel, delegate(DbDataReader reader)
            {
                if (reader.Read())
                {
                    newname = reader.GetString(0);
                    newdesc = reader.GetString(1);
                    return 1;
                }
                else
                    return 0;
            });


            NUnit.Framework.Assert.AreEqual(newname, origname);
            NUnit.Framework.Assert.AreEqual(newdesc, origdesc);
            Console.WriteLine("Values have been restored to the original values of '" + newname + "' and '" + newdesc + "'");

        }

        #region private class Category

        /// <summary>
        /// A simple class for testing strongly typed loading and updating
        /// </summary>
        private class Category
        {
            public int ID;
            public string Name;
            public string Desc;
            public byte[] Image;

            public override string ToString()
            {
                return string.Format("Category (ID:{0}, Name:'{1}', Desc:'{2}', HasImage:{3})", this.ID, this.Name, this.Desc, null != this.Image);
            }

            public Category()
                : this(-1, string.Empty, string.Empty, null)
            {
            }

            public Category(int id, string name, string desc, byte[] image)
            {
                this.ID = id;
                this.Name = name;
                this.Desc = desc;
                this.Image = image;
            }

        }

        #endregion

        /// <summary>
        /// Inserts 2 rows into the database, checks that they are there, then deletes them.
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_06_InsertTest()
        {
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            
            //create the parameters and insert query
            DBParam name = DBParam.ParamWithValue("new name");
            DBParam desc = DBParam.ParamWithValue("new description");

            //create the SQL Statement
            DBInsertQuery insert = DBQuery.InsertInto(Nw.Categories.Table)
                                            .Fields(Nw.Categories.CategoryName, Nw.Categories.Description)
                                            .Values(name, desc);


            //write the SQL to the console
            OutputCommand(db, insert);


            //execute the insert and validate the response
            int count = db.ExecuteNonQuery(insert);
            NUnit.Framework.Assert.AreEqual(count, 1);

            //change the parameter values and run again
            name.Value = "another name";
            desc.Value = "another description";
            count += db.ExecuteNonQuery(insert);

           
            Console.WriteLine("Inserted {0} items into the database", count);


            //check that the values have been stored using the WhereIn construct
            DBQuery sel = DBQuery.SelectCount()
                                .From(Nw.Categories.Table)
                                .WhereIn(Nw.Categories.CategoryName, "new name", "another name");


            //write the SQL to the console
            OutputCommand(db, sel);


            int found = Convert.ToInt32(db.ExecuteScalar(sel));
            

            Console.WriteLine("Found {0} items into the database with the required names", found);

            //clean up by deleting
            DBQuery del = DBQuery.DeleteFrom(Nw.Categories.Table)
                                                    .WhereIn(Nw.Categories.CategoryName, "new name", "another name");
            OutputCommand(db, del);
            
            int deleted = db.ExecuteNonQuery(del);
            NUnit.Framework.Assert.AreEqual(count, deleted);

            Console.WriteLine("Cleaned up {0} items with deletion", deleted);
            
        }

        /// <summary>
        /// Performs a script to insert a record and return the Last ID
        /// within the same operation. NOT Supported in MS Access.
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_07_InsertListAndReturnIDs()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider); 
            
            //create a list of item to insert (in this case 10 items).
            List<Category> toInsert = new List<Category>();
            for (int i = 0; i < 10; i++)
            {
                toInsert.Add(new Category(-1, "new item " + (toInsert.Count + 1).ToString()
                                        , "new description", null));
            }


            int index = 0;

            //again these delegate parameters use an index value external to their definition
            //this time to get a property value from the Category instance
            DBParam cname = DBParam.ParamWithDelegate(delegate { return toInsert[index].Name; });
            DBParam cdesc = DBParam.ParamWithDelegate(delegate { return toInsert[index].Desc; });



            //the script takes an array of DBQuery statements, or use the '.Then' construct
            DBScript script = DBQuery.Begin(
                DBQuery.InsertInto(Nw.Categories.Table)
                                  .Fields(Nw.Categories.CategoryName, Nw.Categories.Description)
                                  .Values(cname, cdesc),
                DBQuery.Select(DBFunction.LastID()) //the provider specific name of this function is catered for in each statement builder.
            );

            //write the SQL to the console
            OutputCommand(db, script);


            //execute this in a transaction
            int count = 0;
            using(DbTransaction transaction = db.BeginTransaction())
            {
                for (index = 0; index < toInsert.Count; index++)
                {
                    Console.WriteLine();
                    //the instances ID property is set to the return id
                    toInsert[index].ID = Convert.ToInt32(db.ExecuteScalar(transaction, script));
                    Console.WriteLine("Inserted : {0}", toInsert[index]);
                    count++;
                }

                transaction.Commit();
            }

            Console.WriteLine("Inserted a total of '{0}' rows", count);

            //clean up by collecting together all the inserted ids and deleting them
            object[] allids = new object[count];
            for (int i = 0; i < count; i++)
            {
                allids[i] = toInsert[i].ID;
            }

            //use the WhereIn construct that can take an object[]
            DBQuery del = DBQuery.DeleteFrom(Nw.Categories.Table)
                                 .WhereIn(Nw.Categories.CategoryID, allids);

            //write the SQL to the console
            OutputCommand(db, del);

            //Execute and get back the number of rows deleted.
            int deleted = db.ExecuteNonQuery(del);

            //assert that they are all deleted
            NUnit.Framework.Assert.AreEqual(deleted, count);
            Console.WriteLine("Deleted '{0}' rows from the table", deleted);


        }

        
        /// <summary>
        /// Creates a sub select query used in a WhereExists construct
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_08_WhereSubSelect()
        {
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider); 

            //look for the used categories with a subselect on products
            DBQuery select = DBQuery.Select().Field(Nw.Categories.CategoryName).As("usedCategory")
                            .From(Nw.Categories.Table)
                            .WhereExists(
                                //sub select joint with category IDs
                                DBQuery.SelectAll()
                                       .From(Nw.Products.Table)
                                       .Where(Nw.Products.Table, Nw.Products.CategoryID, Compare.Equals, Nw.Categories.Table, Nw.Categories.CategoryID))
                            .OrderBy(Nw.Categories.CategoryName);


            //write the SQL to the console
            OutputCommand(db, select);


            //read the results into a list of their names
            List<string> names = new List<string>();

            db.ExecuteRead(select, delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    names.Add(reader.GetString(0));
                }
                return null;
            });


            //output the results of the query
            Console.WriteLine("There are '{0}' categories that have products assigned to them", names.Count);
            Console.Write("These are : ");
            foreach (string name in names)
            {
                Console.Write(name);
                Console.Write(", ");
            }
            Console.WriteLine();

        }


        /// <summary>
        /// Sub select statement with multiple joins.
        /// NOT supported in MS Access. This is as complicated as it should get!
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_09_BigSubSelect()
        {
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);

            object[] requiredregions = new object[] { "SP", "OR" };

            DBSelectQuery customersInRegion = DBQuery.SelectFields(Nw.Customers.CustomerID, Nw.Customers.Region)
                                         .From(Nw.Customers.Table)
                                         .WhereIn(Nw.Customers.Region, requiredregions);

            DBSelectQuery productsoldinregion = DBQuery.Select().Field("OD", Nw.OrderDetails.ProductID)
                                                .Sum("OD", Nw.OrderDetails.Quantity).As("Total Quantity")
                                                .From(Nw.Orders.Table).As("O")
                                                    .InnerJoin(Nw.OrderDetails.Table).As("OD")
                                                        .On("O", Nw.Orders.OrderID, Compare.Equals, "OD", Nw.OrderDetails.OrderID)
                                                    //link in the other query
                                                    .InnerJoin(customersInRegion).As("C")
                                                        .On("O", Nw.Orders.CustomerID, Compare.Equals, "C",Nw.Customers.CustomerID)
                                                .GroupBy("OD", Nw.Products.ProductID)
                                                .OrderBy("Total Quantity", Order.Descending);
                                                

            //output the inner sql
            OutputCommand(db, customersInRegion);
            //output the full sql
            OutputCommand(db, productsoldinregion);


            List<int> prods = new List<int>();
            List<int> qtys = new List<int>();

            db.ExecuteRead(productsoldinregion, delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    object zero = reader[0];
                    object one = reader[1];
                    prods.Add(Convert.ToInt32(zero));//productid
                    qtys.Add(Convert.ToInt32(one));//sum of quantity sold
                }
                return null;
            });

            
            Console.WriteLine();
            Console.WriteLine("Total of products sold in the specified regions");

            for (int i = 0; i < prods.Count; i++)
            {
                Console.WriteLine("ProductID : " + prods[i].ToString() + ", Total Quantity : " + qtys[i]);
            }
            
        }

        /// <summary>
        /// Two select queries executed within the same command script to return
        /// 2 results sets
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_10_MultipleResults()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            Dictionary<int, string> categories = new Dictionary<int, string>();
            Dictionary<int, string> products = new Dictionary<int, string>();

            //first select statement
            DBQuery first = DBQuery.SelectFields(Nw.Categories.CategoryID, Nw.Categories.CategoryName)
                                   .From(Nw.Categories.Table);
            
            //second select statement
            DBQuery second = DBQuery.SelectFields(Nw.Products.ProductID, Nw.Products.ProductName)
                                    .From(Nw.Products.Table);

            //combine the 2 queries into one script
            DBQuery script = DBQuery.Script(first, second);

            //write the SQL to the console
            OutputCommand(db, script);


            //read both results into their respective dictionaries
            db.ExecuteRead(script, delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    categories.Add(Convert.ToInt32(reader[0]), Convert.ToString(reader[1]));
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        products.Add(Convert.ToInt32(reader[0]), Convert.ToString(reader[1]));
                    }
                }
                return null;
            });

            //output the values read from the database
            Console.WriteLine("Categories");
            foreach (int id in categories.Keys)
            {
                Console.Write("{0}:{1}, ", id, categories[id]);
            }

            Console.WriteLine("Products");
            foreach (int id in products.Keys)
            {
                Console.Write("{0}:{1}, ", id, products[id]);
            }
        }


        /// <summary>
        /// Two select queries executed within the same command script to fill a dataset
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_11_FillingDataset()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);
            
            //first select statement
            DBQuery first = DBQuery.SelectFields(Nw.Categories.CategoryID, Nw.Categories.CategoryName)
                                   .From(Nw.Categories.Table);

            //second select statement
            DBQuery second = DBQuery.SelectFields(Nw.Products.ProductID, Nw.Products.ProductName)
                                    .From(Nw.Products.Table);

            //combine the 2 queries into one script
            DBQuery script = DBQuery.Script(first, second);

            //write the SQL to the console
            OutputCommand(db, script);

            DataSet ds = new DataSet();
            ds.Tables.Add("Categories");
            ds.Tables.Add("Products");

            //the default behaviour is to populate the tables in the native order
            //columns do not need to be created because they will be appended if they do not exist
            db.PopulateDataSet(ds, script);

            Console.WriteLine("dataSet contains '" + ds.Tables.Count.ToString() + "' tables");
            
            foreach (DataTable dt in ds.Tables)
            {
                Console.Write("DataTable '" + dt.TableName + "' contains '" + dt.Rows.Count + "' rows\r\n");
                Console.WriteLine("Columns:{0},{1}", dt.Columns[0], dt.Columns[1]);

                foreach (DataRow dr in dt.Rows)
                {
                    Console.Write("Row:");
                    Console.Write("{0},{1}\r\n", dr.ItemArray);
                }
                Console.WriteLine();
            }

            
        }

        /// <summary>
        /// Checks the TopN and TopPercent constructs against the dataprovider
        /// </summary>
        [NUnit.Framework.Test()]
        public void Northwind_12_SelectTopN()
        {
            //initialize
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);

            //select statements - count, top 4 and top 10 percent
            DBQuery count = DBQuery.SelectCount().From(Nw.OrderDetails.Table);

            DBQuery topfour = DBQuery.Select().TopN(4)
                                            .Fields(Nw.OrderDetails.OrderID, Nw.OrderDetails.Quantity)
                                            .From(Nw.OrderDetails.Table)
                                            .OrderBy(Nw.OrderDetails.Quantity, Order.Descending).And(Nw.OrderDetails.OrderID);

            DBQuery toppercent = DBQuery.Select().TopPercent(10)
                                            .Fields(Nw.OrderDetails.OrderID, Nw.OrderDetails.Quantity)
                                            .From(Nw.OrderDetails.Table)
                                            .OrderBy(Nw.OrderDetails.Quantity, Order.Descending).And(Nw.OrderDetails.OrderID);
             
            //write the SQL to the console
            OutputCommand(db, count);
            OutputCommand(db, topfour);
            OutputCommand(db, toppercent);

            //gets the total count of rows in the order details table so we can 
            //compare later on
            int cnt = Convert.ToInt32(db.ExecuteScalar(count));

            //fill and return an array of the ids of the top items
            int[] topids = (int[])db.ExecuteRead(topfour, delegate(DbDataReader reader)
            {
                List<int> ids = new List<int>();
                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader[0]));                    
                }
                return ids.ToArray();
            });
            //validate the result
            NUnit.Framework.Assert.AreEqual(4, topids.Length);


            int[] percent = (int[])db.ExecuteRead(toppercent, delegate(DbDataReader reader)
            {
                List<int> ids = new List<int>();
                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader[0]));
                }
                return ids.ToArray();
            });

            //check that the values are the same on both
            for (int i = 0; i < topids.Length; i++)
			{
                NUnit.Framework.Assert.AreEqual(topids[i],percent[i]);
			}
            
            //now confirm the percent returned is within hte bounds of the total number of rows
            double required = (cnt / 100.0) * 10.0;
            int min = (int)Math.Floor(required);
            int max = (int)Math.Ceiling(required);

            NUnit.Framework.Assert.IsTrue(percent.Length >= min && percent.Length <= max);

            Console.WriteLine("Total row count was :{0}, 10 percent of this was returned:{1}, " +
                "and the ids of the four with the highest quantity were:{2},{3},{4},{5}", cnt, percent.Length, topids[0], topids[1], topids[2], topids[3]);

        }



        //
        // Support methods
        //

        /// <summary>
        /// Outputs the SQL for a DBQuery onto the console, along with any parameters
        /// </summary>
        /// <param name="db"></param>
        /// <param name="q"></param>
        private static void OutputCommand(DBDatabase db, DBQuery q)
        {
            Console.WriteLine();
            using (DbCommand cmd = db.CreateCommand(q))
            {
                Console.WriteLine(cmd.CommandText);
                if (cmd.Parameters.Count > 0)
                {
                    foreach (DbParameter p in cmd.Parameters)
                    {
                        Console.WriteLine("Parameter : {0} ( Type:{1}, Direction:{2}, Size:{3}, Source:{4}, Value:{5})", p.ParameterName, p.DbType, p.Direction, p.Size, p.SourceColumn, p.Value);

                    }
                }
            }
        }

    }
}
