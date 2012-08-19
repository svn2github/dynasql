using System;
using System.Collections.Generic;
using System.Text;
using Perceiveit.Data;
using Perceiveit.Data.Query;
using System.Data.Common;
using System.Data;
using Microsoft.VisualStudio.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Perceiveit.Data.DynaSql.Tests
{
    /// <summary>
    /// Tests the generation of SQL Statements against a specific database provider.
    /// All queries use the Northwind database schema - defined in the NorthwindSchema.cs <see cref="Perceiveit.Data.DynaSql.Tests.Nw"/> file.
    /// </summary>
    /// <remarks>Each database schema I could find for Northwind has slightly different names
    /// so the Nw class has comment sections to use these specific names</remarks>
    [TestClass]
    public class DynaSQLTests
    {
        #region public TestContext TestContext {get;set;}

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        [TestInitialize]
        public void SetUpConnectionDb()
        {
            //set up is called once for the test suite execution. Here we 
            //output the connection properties we are using just for identification purposes
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            TestContext.WriteLine("DataBase Properties: {0} ", db.GetProperties().ToString());
        }

        /// <summary>
        /// Just checks the execution of a Select Statements
        /// </summary>
        [TestMethod()]
        public void Northwind_00_SelectOrders()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);
            DBQuery sel = DBQuery.SelectAll().From("DBUSER",Nw.Orders.Table);
            int count = 0;

            TestContext.WriteLine(sel.ToSQLString(db));
            db.ExecuteRead(sel, reader =>
            {
                while (reader.Read())
                {
                    count++;
                }
            });

            TestContext.WriteLine("read {0} rows from the database", count);

        }

        /// <summary>
        /// Extracts a count of all the orders for all customers after a specified date with a parameter
        /// </summary>
        [TestMethod]
        public void Northwind_01_OrderCountTest()
        {
            TestContext.WriteLine("BEGINNING TEST 01");
            
            //initialize
            StringBuilder names = new StringBuilder();
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);
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
            
            TestContext.WriteLine("Total read time : " + sw.Elapsed.ToString());
            TestContext.WriteLine("Count for Customers = {0}", count);
            TestContext.WriteLine("Customers : " + names.ToString());
            
        }

        /// <summary>
        /// Extracts all the orders with the number of Order Detail items for a
        /// specific customer - specified via a parameter
        /// </summary>
        [TestMethod]
        public void Northwind_02_OrderToOrderDetails()
        {
            TestContext.WriteLine("BEGINNING TEST 02");

            //initialize
            StringBuilder names = new StringBuilder();
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);
            DBStatementBuilder builder = db.CreateStatementBuilder();
            builder.GetUniqueParameterName();

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            //A static parameter with the specified value
            DBParam cid;
            if (Nw.DbProvider == "System.Data.OleDb") 
                cid = DBParam.ParamWithValue(1); //My access schema uses integers for hte Customer ID
            else
                cid = DBParam.ParamWithValue("BERGS");
            

            //Create the select query
            DBQuery sel = DBQuery.Select().Field(Nw.Orders.CustomerID)
                                          .Field("O", Nw.Orders.OrderID)
                                          .Sum("OD",Nw.OrderDetails.Quantity).As("itemCount")
                                   .From(Nw.Schema, Nw.Orders.Table).As("O")
                                        .InnerJoin(Nw.Schema, Nw.OrderDetails.Table).As("OD")
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
            TestContext.WriteLine("Total read time : " + sw.Elapsed.ToString());
            TestContext.WriteLine("Count for Customer Orders = {0}", count);
            Console.Write("Item count for customer '" + cid.Value.ToString() + "' orders :");
            TestContext.WriteLine(names.ToString());
        }

        /// <summary>
        /// Checks the wherein construct against an array of integers
        /// </summary>
        [TestMethod]
        public void Northwind_03_ExistsInTest()
        {
            TestContext.WriteLine("BEGINNING TEST 03");

            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            int expected = 0;

            //Execute for each value 
            DBParam value = DBParam.ParamWithValue(0);

            DBQuery one = DBQuery.SelectCount().From(Nw.Categories.Table)
                                               .WhereFieldEquals(Nw.Categories.CategoryID, value);
            for (int i = 1; i < 5; i++)
            {
                value.Value = i;
                expected += Convert.ToInt32(db.ExecuteScalar(one));
            }

            //Now execute a Where in 
            DBSelectQuery select = DBQuery.SelectCount()
                                          .From(Nw.Categories.Table)
                                          .WhereIn(Nw.Categories.CategoryID, 1, 2, 3, 4);

            //write the SQL to the console
            OutputCommand(db, select);

            int count = Convert.ToInt32(db.ExecuteScalar(select));
            Assert.AreEqual(expected, count);

            //output the results of the query
            TestContext.WriteLine("There are '{0}' rows with ids of '1,2,3 or 4'", count);

        }


        /// <summary>
        /// Executes the sames statement multiple times based on the array
        /// of integers for product ids.
        /// </summary>
        [TestMethod]
        public void Northwind_04_ArrayTestWithDelegate()
        {
            TestContext.WriteLine("BEGINNING TEST 04");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db); 
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
                    //TestContext.WriteLine("Get order id from the array for index '" + index + "' = " + orderids[index]);
                    
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
            TestContext.WriteLine("Results from Collection Test");
            TestContext.WriteLine("ids .count = " + ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                TestContext.WriteLine("{0}: productID={1}, count={2}, sum={3}", i, ids[i], counts[ids[i]], sums[ids[i]]);
            }
                
        }

        /// <summary>
        /// Updates the name and description of a category and checks that the name was updated.
        /// Finally resetting it to its previous value
        /// </summary>
        [TestMethod]
        public void Northwind_05_UpdateTests()
        {
            TestContext.WriteLine("BEGINNING TEST 05");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db); 
            
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
                TestContext.WriteLine("Updated '" + updated.ToString() + "' rows with the new values");

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
                Assert.AreEqual(newname, "test 2");
                Assert.AreEqual(newdesc, "this is the new description");
                TestContext.WriteLine("Values have been changed to the new values of '" + newname + "' and '" + newdesc + "'");


                //throw an exception and confirm that the action was rolled back in the transaction
                //or comment out to fully execute the resetting of the values
                throw new ArgumentException("Intentional exception");


                //restore the previous values by setting the parameter values
                cname.Value = origname;
                cdesc.Value = origdesc;
                updated = db.ExecuteNonQuery(transaction, update);
                TestContext.WriteLine("Updated '" + updated.ToString() + "' rows with the original values");


                //commit the transaction as successfully executed
                transaction.Commit();
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "Intentional exception")
                    TestContext.WriteLine("Caught and sank the intentional exception");//this should be sunk
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


            Assert.AreEqual(newname, origname);
            Assert.AreEqual(newdesc, origdesc);
            TestContext.WriteLine("Values have been restored to the original values of '" + newname + "' and '" + newdesc + "'");

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
        [TestMethod]
        public void Northwind_06_InsertTest()
        {
            TestContext.WriteLine("BEGINNING TEST 06");

            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

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
            Assert.AreEqual(count, 1);

            //change the parameter values and run again
            name.Value = "another name";
            desc.Value = "another description";
            count += db.ExecuteNonQuery(insert);

           
            TestContext.WriteLine("Inserted {0} items into the database", count);


            //check that the values have been stored using the WhereIn construct
            DBQuery sel = DBQuery.SelectCount()
                                .From(Nw.Categories.Table)
                                .WhereIn(Nw.Categories.CategoryName, "new name", "another name");


            //write the SQL to the console
            OutputCommand(db, sel);


            int found = Convert.ToInt32(db.ExecuteScalar(sel));
            

            TestContext.WriteLine("Found {0} items into the database with the required names", found);

            //clean up by deleting
            DBQuery del = DBQuery.DeleteFrom(Nw.Categories.Table)
                                                    .WhereIn(Nw.Categories.CategoryName, "new name", "another name");
            OutputCommand(db, del);
            
            int deleted = db.ExecuteNonQuery(del);
            Assert.AreEqual(found, deleted);

            TestContext.WriteLine("Cleaned up {0} items with deletion", deleted);
            
        }

        /// <summary>
        /// Performs a script to insert a record and return the Last ID
        /// within the same operation. NOT Supported in MS Access.
        /// </summary>
        [TestMethod]
        public void Northwind_07_InsertListAndReturnIDs()
        {
            TestContext.WriteLine("BEGINNING TEST 07");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

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
            DBConst name = DBConst.String(toInsert[index].Name);
            DBConst desc = DBConst.String(toInsert[index].Desc);


            //the script takes an array of DBQuery statements, or use the '.Then' construct
            DBScript script = DBQuery.Begin(
                DBQuery.InsertInto(Nw.Schema, Nw.Categories.Table)
                                  .Fields(Nw.Categories.CategoryName, Nw.Categories.Description)
                                  .Values(cname, cdesc),
                DBQuery.Select(DBFunction.LastID())
            );

            

            
            if (db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts))
            {
                //write the SQL to the console
                OutputCommand(db, script);


                //execute this in a transaction
                int count = 0;
                using (DbTransaction transaction = db.BeginTransaction())
                {
                    for (index = 0; index < toInsert.Count; index++)
                    {
                        //the instances ID property is set to the return id
                        toInsert[index].ID = Convert.ToInt32(db.ExecuteScalar(transaction, script));
                        TestContext.WriteLine("Inserted : {0}", toInsert[index]);
                        count++;
                    }

                    transaction.Commit();
                }

                TestContext.WriteLine("Inserted a total of '{0}' rows", count);

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
                Assert.AreEqual(deleted, count);
                TestContext.WriteLine("Deleted '{0}' rows from the table", deleted);

            }
            else
            {
                TestContext.WriteLine("Skipped DBScrips as this is not supported by the provider");
            }
        }

        

        
        /// <summary>
        /// Creates a sub select query used in a WhereExists construct
        /// </summary>
        [TestMethod]
        public void Northwind_08_WhereSubSelect()
        {
            TestContext.WriteLine("BEGINNING TEST 08");

            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            //look for the used categories with a subselect on products
            DBQuery select = DBQuery.Select().Field(Nw.Categories.CategoryName).As("usedCategory")
                            .From(Nw.Schema,Nw.Categories.Table)
                            .WhereExists(
                                //sub select joint with category IDs
                                DBQuery.SelectAll()
                                       .From(Nw.Schema,Nw.Products.Table).As("P")
                                       .Where("P", Nw.Products.CategoryID, Compare.Equals, Nw.Categories.Table, Nw.Categories.CategoryID))
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
            TestContext.WriteLine("There are '{0}' categories that have products assigned to them", names.Count);
            Console.Write("These are : ");
            foreach (string name in names)
            {
                Console.Write(name);
                Console.Write(", ");
            }
            

        }


        /// <summary>
        /// Sub select statement with multiple joins.
        /// NOT supported in MS Access. This is as complicated as it should get!
        /// </summary>
        [TestMethod]
        public void Northwind_09_BigSubSelect()
        {
            TestContext.WriteLine("BEGINNING TEST 09");

            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            object[] requiredregions = new object[] { "SP", "OR" };

            DBSelectQuery customersInRegion = DBQuery.SelectFields(Nw.Customers.CustomerID, Nw.Customers.Region)
                                         .From(Nw.Schema,Nw.Customers.Table)
                                         .WhereIn(Nw.Customers.Region, requiredregions);

            DBSelectQuery productsoldinregion = DBQuery.Select().Field("OD", Nw.OrderDetails.ProductID)
                                                .Sum("OD", Nw.OrderDetails.Quantity).As("Total Quantity")
                                                .From(Nw.Schema,Nw.Orders.Table).As("O")
                                                    .InnerJoin(Nw.Schema, Nw.OrderDetails.Table).As("OD")
                                                        .On("O", Nw.Orders.OrderID, Compare.Equals, "OD", Nw.OrderDetails.OrderID)
                                                    //link in the other query
                                                    .InnerJoin(customersInRegion).As("C")
                                                        .On("O", Nw.Orders.CustomerID, Compare.Equals, "C",Nw.Customers.CustomerID)
                                                .GroupBy("OD", Nw.Products.ProductID)
                                                .OrderBy("Total Quantity", Order.Descending);
                                                

            //output the inner sql
            //OutputCommand(db, customersInRegion);
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

            
            TestContext.WriteLine("Total of products sold in the specified regions");

            for (int i = 0; i < prods.Count; i++)
            {
                TestContext.WriteLine("ProductID : " + prods[i].ToString() + ", Total Quantity : " + qtys[i]);
            }
            
        }

        /// <summary>
        /// Two select queries executed within the same command script to return
        /// 2 results sets
        /// </summary>
        [TestMethod]
        public void Northwind_10_MultipleResults()
        {
            

            TestContext.WriteLine("BEGINNING TEST 10");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            if (db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts) == false)
            {
                TestContext.WriteLine("Skipping test as scripts are not supported");
                return;
            }

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
            TestContext.WriteLine("Categories");
            foreach (int id in categories.Keys)
            {
                Console.Write("{0}:{1}, ", id, categories[id]);
            }

            TestContext.WriteLine("Products");
            foreach (int id in products.Keys)
            {
                Console.Write("{0}:{1}, ", id, products[id]);
            }
        }
    


        /// <summary>
        /// Two select queries executed within the same command script to fill a dataset
        /// </summary>
        [TestMethod]
        public void Northwind_11_FillingDataset()
        {
            TestContext.WriteLine("BEGINNING TEST 11");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

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
            if (db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts))
            {
                db.PopulateDataSet(ds, script);
            }
            else
            {
                db.PopulateDataSet(ds, first, LoadOption.OverwriteChanges, "Categories");
                db.PopulateDataSet(ds, second, LoadOption.OverwriteChanges, "Products");
            }

            TestContext.WriteLine("dataSet contains '" + ds.Tables.Count.ToString() + "' tables");
            
            foreach (DataTable dt in ds.Tables)
            {
                Console.Write("DataTable '" + dt.TableName + "' contains '" + dt.Rows.Count + "' rows\r\n");
                TestContext.WriteLine("Columns:{0},{1}", dt.Columns[0], dt.Columns[1]);

                foreach (DataRow dr in dt.Rows)
                {
                    Console.Write("Row:");
                    Console.Write("{0},{1}\r\n", dr.ItemArray);
                }
                
            }

            
        }

        /// <summary>
        /// Checks the TopN and TopPercent constructs against the dataprovider
        /// </summary>
        [TestMethod]
        public void Northwind_12_SelectTopN()
        {
            TestContext.WriteLine("BEGINNING TEST 12");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind",Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

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

            DBSelectQuery sub = DBQuery.Select().TopN(100)
                                                .Sum(Nw.OrderDetails.Quantity).As("total")
                                                .Field(Nw.OrderDetails.ProductID).As("prod")
                                                .From(Nw.OrderDetails.Table)
                                                .GroupBy(Nw.OrderDetails.ProductID)
                                                .OrderBy(DBAggregate.Aggregate(AggregateFunction.Sum, DBField.Field(Nw.OrderDetails.Quantity)), Order.Descending);

            DBQuery topwithSubSelect = DBQuery.Select().TopN(10)
                                            .Fields("total", "prod")
                                            .From(sub).As("sub")
                                            .OrderBy("total", Order.Descending);

            DBQuery toppage = DBQuery.Select().TopRange(4, 5)
                                              .Fields("total", "prod")
                                              .From(sub).As("sub")
                                              .OrderBy("total", Order.Descending);
            //write the SQL to the console

            bool supportsPercent = false;

            TopType[] all = db.GetProperties().SupportedTopTypes;
            if (Array.IndexOf<TopType>(all, TopType.Percent) > -1)
                supportsPercent = true;

            OutputCommand(db, count);
            OutputCommand(db, topfour);

            if (supportsPercent)
                OutputCommand(db, toppercent);
            else
                TestContext.WriteLine("Not executing Top Percent as it is not supported in the Provider");

            OutputCommand(db, topwithSubSelect);

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
            Assert.AreEqual(4, topids.Length);
            int[] percent = new int[] { };

            if (supportsPercent)
            {
                percent = (int[])db.ExecuteRead(toppercent, delegate(DbDataReader reader)
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
                    Assert.AreEqual(topids[i], percent[i], "The id's at index '" + i + "' do not match");
                }
            }

            int[] withsubselect = (int[])db.ExecuteRead(topwithSubSelect, delegate(DbDataReader reader)
            {
                List<int> ids = new List<int>();
                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader[0]));
                }
                return ids.ToArray();
            });
            Assert.AreEqual(withsubselect.Length, 10, "The expected return of 10 products was actually '" + withsubselect.Length.ToString() + "'");

            TestContext.WriteLine("Total row count was :{0}, 10 percent of this was returned:{1}, " +
                "and the ids of the four with the highest quantity were:{2},{3},{4},{5}", cnt, percent.Length, topids[0], topids[1], topids[2], topids[3]);

        }

       

        [TestMethod]
        public void Northwind_13_SelectLeftAndRightJoin()
        {
            TestContext.WriteLine("BEGINNING TEST 13");

            //initialize
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            //Count all the employees then run a left join onto orders subquery (with a date limit) and make sure we get the same number
            //SELECT E.employeeid, O.ordercount FROM EMPLOYEES E
            //    LEFT OUTER JOIN (SELECT employeeid, COUNT(*) as ordercount FROM ORDERS
            //                     WHERE OrderDate > '01 May 1998'
            //                     GROUP BY employeeid) O 
            //        ON E.EmployeeID = O.EmployeeID
            //ORDER BY ordercount


            DateTime dtlimit = new DateTime(1998, 05, 01);

            DBQuery allemps = DBQuery.SelectCount().From(Nw.Employees.Table);
            int countallemps = Convert.ToInt32(db.ExecuteScalar(allemps));
            TestContext.WriteLine("Total number of employees is '" + countallemps + "'");

            //Generate the sub query first
            DBSelectQuery empordercount = DBQuery.Select()
                                           .Field(Nw.Orders.EmployeeID)
                                           .Count(DBField.AllFields()).As("ordercount")
                                           .From(Nw.Orders.Table)
                                           .Where(Nw.Orders.OrderDate, Compare.GreaterThan, DBConst.DateTime(dtlimit))
                                           .GroupBy(Nw.Orders.EmployeeID);

            DBSelectQuery left = DBQuery.Select()
                                  .Field("E", Nw.Employees.EmployeeID)
                                  .Field("O", "ordercount")
                                  .From(Nw.Employees.Table).As("E")
                                  .LeftJoin(empordercount).As("O")
                                        .On("E",Nw.Employees.EmployeeID,Compare.Equals,"O",Nw.Orders.EmployeeID)
                                  .OrderBy("O", "ordercount");


            bool hasempty = false;
            Dictionary<int, int> empstoorders = new Dictionary<int, int>();
            //do the read and populate the dictionary
            db.ExecuteRead(left, delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    int empid = reader.GetInt32(0);//employeeid
                    int count;
                    if (reader.IsDBNull(1))
                    {
                        count = -1;
                        hasempty = true;
                    }
                    else
                    {
                        count = reader.GetInt32(1);//ordercount
                    }
                    empstoorders.Add(empid, count);//add to the dictionary
                }
            });

            //make sure empty ones are returned
            if (hasempty)
                TestContext.WriteLine("Left joins found");
            else
                throw new InvalidOperationException("The left join could not be checked - all employees returned orders with in the date");

            Assert.AreEqual(countallemps,empstoorders.Count,"the number of employees returned from the left join does not match the total numer of employees");

            TestContext.WriteLine("Left join returned '" + empstoorders.Count + "' employees which matches the total number of employees");
            foreach (KeyValuePair<int,int> kvp in empstoorders)
            {
                TestContext.WriteLine(string.Format("Employee id '{0}' returned '{1}' orders within the date", kvp.Key, kvp.Value < 0 ? "NULL" : kvp.Value.ToString()));
            }

            //Now Switch it around for the RIGHT OUTER JOIN - just as a test

            //SELECT O.ordercount,  E.employeeid
            //    FROM (SELECT employeeid, COUNT(*) as ordercount FROM ORDERS
            //                     WHERE OrderDate > '01 May 1998'
            //                     GROUP BY employeeid) O 
            //    RIGHT OUTER JOIN EMPLOYEES E
            //        ON O.EmployeeID = E.EmployeeID
            //ORDER BY ordercount


            DBSelectQuery right = DBQuery.Select()
                                  .Field("O", "ordercount")
                                  .Field("E", Nw.Employees.EmployeeID)
                                  .From(empordercount).As("O")
                                  .RightJoin(Nw.Employees.Table).As("E")
                                        .On("O", Nw.Orders.EmployeeID, Compare.Equals, "E", Nw.Employees.EmployeeID)
                                  .OrderBy("O", "ordercount");


            hasempty = false;
            Dictionary<int, int> orderstoemps = new Dictionary<int, int>();
            //do the read and populate the dictionary
            db.ExecuteRead(right, delegate(DbDataReader reader)
            {
                while (reader.Read())
                {
                    int count;
                    if (reader.IsDBNull(0))
                    {
                        count = -1;
                        hasempty = true;
                    }
                    else
                    {
                        count = reader.GetInt32(0);//ordercount
                    }

                    int empid = reader.GetInt32(1);//employeeid
                    orderstoemps.Add(empid, count);//add to the dictionary
                }
            });

            //make sure empty ones are returned
            if (hasempty)
                TestContext.WriteLine("Right joins found");
            else
                throw new InvalidOperationException("The right join could not be checked - all employees returned orders with in the date");

            Assert.AreEqual(countallemps, orderstoemps.Count, "the number of employees returned from the RIGHT join does not match the total numer of employees");

            TestContext.WriteLine("RIGHT join returned '" + orderstoemps.Count + "' employees which matches the total number of employees");
            foreach (KeyValuePair<int, int> kvp in orderstoemps)
            {
                TestContext.WriteLine(string.Format("Employee id '{0}' returned '{1}' orders within the date", kvp.Key, kvp.Value < 0 ? "NULL" : kvp.Value.ToString()));
            }

        }

        [TestMethod]
        public void Northwind_14_StoredProcedure()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            //Execute a stored procedure with a single parameter
            DBQuery exec = DBQuery.Exec("CustOrderHist")
                                  .WithParamValue("CustomerID", DbType.String, "ALFKI");

            Dictionary<string, int> totals = new Dictionary<string, int>();

            db.ExecuteRead(exec, delegate(DbDataReader reader)
            {
                int nameOrd = reader.GetOrdinal("ProductName");
                int totalOrd = reader.GetOrdinal("Total");
                while (reader.Read())
                {
                    string name = reader.GetString(nameOrd);
                    int total = reader.GetInt32(totalOrd);

                    totals.Add(name, total);
                }
            });

            List<string> names = new List<string>(totals.Keys);
            names.Sort();

            const int MaxLength = 50;

            Console.Write("Product".PadRight(MaxLength));
            TestContext.WriteLine(" Total");
            foreach (string name in names)
            {
                if (name.Length > 50)
                {
                    Console.Write(name.Substring(MaxLength - 3));
                    Console.Write("...");
                }
                else
                    Console.Write(name.PadRight(MaxLength));
                Console.Write(" ");

                TestContext.WriteLine(totals[name].ToString());

            }
        }


        
        

        [TestMethod]
        public void Northwind_15_FullyQualifiedReferences()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            //TODO: Implement a full reference.
            DBQuery sel = DBQuery.SelectAll().From(DBTable.Table(Nw.Customers.Table));



        }


        [TestMethod]
        public void Northwind_16_NotClause()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);


            DBQuery sel = DBQuery.SelectAll()
                                 .From(Nw.Customers.Table)
                                 .Where(
                                        DBComparison.Not(
                                            DBComparison.Compare(DBField.Field(Nw.Customers.CompanyName), Compare.Like, DBConst.String("ABC%"))));

        }

       


        [TestMethod]
        public void Northwind_19_Concatentate()
        {
            DBQuery concat = DBQuery.Select(DBField.Field("FirstName") + DBConst.Const(" ") + DBField.Field("LastName")).As("FullName")
                                .From("Employees");

            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            db.ExecuteRead(concat, reader =>
            {
                while (reader.Read())
                {
                    Console.Write("FullName: ");
                    TestContext.WriteLine(reader.GetString(0));
                }
            });

        }


        [TestMethod]
        public void Northwind_17_MultiParamSproc()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            Dictionary<string, decimal> totals = new Dictionary<string, decimal>();

            DBConst begin = DBConst.DateTime(1900, 1, 1);
            DBConst end = DBConst.DateTime(2012, 1, 1);

            //Execute a stored procedure with a single parameter
            DBQuery exec = DBQuery.Exec("Employee Sales by Country")
                                  .WithParamValue("Beginning_Date", begin)
                                  .WithParamValue("Ending_Date", end);
            decimal grandtotal = 0;
            db.ExecuteRead(exec, reader =>
             {
                 int amountindex = reader.GetOrdinal("SaleAmount");

                 while (reader.Read())
                 {
                     decimal amount = reader.GetDecimal(amountindex);
                     grandtotal += amount;
                 }

             });

             TestContext.WriteLine("Grand Total:{0}", grandtotal);
        }


        [TestMethod]
        public void Northwind_18_MultipleComparisonJoin()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            DBComparison country = DBComparison.Equal(DBField.Field(Nw.Orders.ShipCountry),DBConst.String("France"));
            DBComparison country2 = DBComparison.Equal(DBField.Field(Nw.Orders.ShipCountry), DBConst.String("Germany"));
            DBComparison country3 = DBComparison.Equal(DBField.Field(Nw.Orders.ShipCountry), DBConst.String("Spain"));
            DBComparison country4 = DBComparison.Equal(DBField.Field(Nw.Orders.ShipCountry), DBConst.String("Italy"));
            DBComparison shipdate = DBComparison.Compare(DBField.Field(Nw.Orders.ShippedDate), Compare.LessThan, DBConst.DateTime(DateTime.Now));
            DBComparison shipvia = DBComparison.Equal(DBField.Field(Nw.Orders.ShippedVia), DBConst.Int32(3));

            //where all

            DBQuery sel = DBQuery.SelectCount()
                                .From(Nw.Orders.Table)
                                .WhereAll(country, country2, country3, country4);

            int count = (int)db.ExecuteScalar(sel);

            // and where all

            sel = DBQuery.SelectCount()
                .From(Nw.Orders.Table)
                .WhereField(Nw.Orders.ShippedDate, Compare.Is, DBConst.Null())
                .AndWhereAll(country, country2, country3, country4);

            count = (int)db.ExecuteScalar(sel);

            // where none

            sel = DBQuery.SelectCount()
                        .From(Nw.Orders.Table)
                        .WhereNone(country, country2, country3, country4);

            count = (int)db.ExecuteScalar(sel);

            // and where none

            sel = DBQuery.SelectCount()
                .From(Nw.Orders.Table)
                .WhereField(Nw.Orders.ShippedDate, Compare.Is, DBConst.Null())
                .AndWhereNone(country, country2, country3, country4);

            // where all

            count = (int)db.ExecuteScalar(sel);

            sel = DBQuery.SelectCount()
                        .From(Nw.Orders.Table)
                        .WhereAll(country, shipdate, shipvia);

            count = (int)db.ExecuteScalar(sel);

            // and where all

            sel = DBQuery.SelectCount()
                         .From(Nw.Orders.Table)
                         .Where(Nw.Orders.CustomerID, Compare.Equals, DBConst.String("LAMAI"))
                         .AndWhereAll(country, shipvia, shipdate);

            count = (int)db.ExecuteScalar(sel);



        }


        [TestMethod]
        public void Northwind_21_SelectRange()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);
            int startindex = 6;
            int count = 20;

            //First load the TopN so that we can compare with the range
            //and make sure the results are the same from matching offsets
            DBQuery sel = DBQuery.Select().TopN(count)
                                          .Fields(Nw.Orders.OrderID, Nw.Orders.CustomerID)
                                          .From(Nw.Orders.Table)
                                          .Where(Nw.Orders.ShipCountry, Compare.Equals, DBConst.String("France"))
                                          .OrderBy(Nw.Orders.OrderDate, Order.Descending);

            TestContext.WriteLine("Just top 10");
            TestContext.WriteLine(sel.ToSQLString(db));

            //The indexed list of ids that can be compared
            List<int> idlookup = new List<int>();

            db.ExecuteRead(sel, reader =>
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader[0]);
                    idlookup.Add(id);
                }
            });
            Assert.AreEqual(count, idlookup.Count); //Make sure we have all of them

            sel = DBQuery.Select().TopRange(startindex, count)
                                          .Fields(Nw.Orders.OrderID, Nw.Orders.CustomerID)
                                          .From(Nw.Orders.Table)
                                          .Where(Nw.Orders.ShipCountry, Compare.Equals, DBConst.String("France"))
                                          .OrderBy(Nw.Orders.OrderDate, Order.Descending);

            TestContext.WriteLine("With Range select from {0} to {2} inclusive, count {1}", startindex, count, (startindex - 1) + count);
            TestContext.WriteLine(sel.ToSQLString(db));

            db.ExecuteRead(sel, reader =>
            {
                int offset = startindex - 1;//cannot compare 1 based against zero based, so take off one.

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader[0]);
                    if (offset < idlookup.Count)
                    {
                        //Compare the postion in the lookup list (offset is 1 based)
                        Assert.AreEqual(offset, idlookup.IndexOf(id));
                    }
                    else //Make sure it is not found
                        Assert.AreEqual(-1, idlookup.IndexOf(id));
                    offset++;
                }
            });
        }

        [TestMethod]
        public void Northwind_20_ScriptTest()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            DBParam pid = DBParam.ParamWithDelegate(delegate { return "MYCOM"; });
            DBParam pname = DBParam.ParamWithDelegate(delegate { return "My Company Name"; });

            DBQuery script = DBQuery.Begin(
                        DBQuery.InsertInto(Nw.Customers.Table).Fields(Nw.Customers.CustomerID, Nw.Customers.CompanyName)
                                                      .Values(pid, pname),

                        DBQuery.Select(DBFunction.LastID()).As("identity"),

                        DBQuery.DeleteFrom(Nw.Customers.Table)
                                            .WhereFieldEquals(Nw.Customers.CustomerID, pid)
                                            .AndWhereField(Nw.Customers.CompanyName, Compare.Equals, pname),

                        DBQuery.DeleteFrom(Nw.Customers.Table)
                                            .WhereFieldEquals(Nw.Customers.CustomerID, pid)
                                            .AndWhereField(Nw.Customers.CompanyName, Compare.Equals, pname))
                        .End();
                       
            TestContext.WriteLine(script.ToSQLString(db));
            object value = db.ExecuteScalar(script);
            TestContext.WriteLine("Returned identifier was {0}", value);
            
        }

        [TestMethod()]
        public void Northwind_21_TableHints()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            DBQuery hinted = DBQuery.Select()
                                            .Field(Nw.Customers.Table, Nw.Customers.CustomerID)
                                            .Field("O", Nw.Orders.OrderID)
                                    .From(Nw.Customers.Table)
                                                    .WithHints(DBTableHint.NoLock, DBTableHint.NoWait)
                                                    .WithHint(DBTableHint.Index, "pk_Customers")
                                            .InnerJoin(Nw.Orders.Table).As("O").WithHint(DBTableHint.ForceSeek)
                                    .On(Nw.Customers.Table, Nw.Customers.CustomerID, Compare.Equals, "O", Nw.Orders.CustomerID);

            string sql = hinted.ToSQLString(db);
            TestContext.WriteLine(sql);

            int count = 0;
            db.ExecuteRead(hinted, reader =>
            {
                while (reader.Read())
                {
                    //Just junk to make sure we read
                    string cid = reader[0].ToString();
                    string oid = reader[1].ToString();
                    count++;
                }
            });

            TestContext.WriteLine("Loaded {0} customer orders ", count);
        }


        [TestMethod]
        public void AnyOldSql()
        {
            DBDatabase db = DBDatabase.Create("Northwind", Nw.DbConnection, Nw.DbProvider);
            AttachProfiler(db);

            DBParam condition = DBParam.ParamWithValue(string.Format("%{0}%", "A"));
            DBQuery sel = DBQuery.SelectCount()
                                 .From(Nw.Customers.Table)
                                  .WhereField(Nw.Customers.CustomerID, Compare.Like, condition);

            db.ExecuteRead(sel, reader =>
            {
                while (reader.Read())
                {
                    int count = reader.GetInt32(0);
                    TestContext.WriteLine("Found '" + count.ToString() + "' records matching '" + condition + "'");
                }
            });

            var tableName = "tableName";

            Dictionary<string, object> assignValues = new Dictionary<string, object>();
            assignValues.Add("column1", 1);
            assignValues.Add("column2", 2);

            Dictionary<string, object> whereValues = new Dictionary<string, object>();
            whereValues.Add("column1", 1);
            whereValues.Add("column2", 2);

            var qry = DBQuery.Update(tableName);

            //set
            foreach (KeyValuePair<string, object> kvp in assignValues)
            {
                qry.Set(kvp.Key, DBParam.ParamWithValue(kvp.Value));
            }

            //where
            bool first = true;

            foreach (KeyValuePair<string,object> kvp in whereValues)
            {
                if (first)	
                    qry.WhereField(kvp.Key, Compare.Equals, DBParam.ParamWithValue(kvp.Value));
                else
                    qry.AndWhere(kvp.Key,Compare.Equals,DBParam.ParamWithValue(kvp.Value));
                first = false;
            }

            string sql = qry.ToSQLString(db);
            TestContext.WriteLine(sql);
 
        }




        [TestCleanup]
        public void DumpSummary()
        {
            Profile.DBConsoleProfilerSingletonFactory.Instance().DumpExecutionSummary();
        }

        //
        // Support methods
        //

        private static void AttachProfiler(DBDatabase db)
        {
            //Profilers are defined in the config file - if not using then can be attached here as required
            //db.AttachProfiler(Profile.DBConsoleProfilerSingletonFactory.Instance(), true);
            //db.AttachProfiler(new Profile.DBTextFileProfiler("Northwind"), true);
        }

        /// <summary>
        /// Outputs the SQL for a DBQuery onto the console, along with any parameters
        /// </summary>
        /// <param name="db"></param>
        /// <param name="q"></param>
        private void OutputCommand(DBDatabase db, DBQuery q)
        {
            using (DbCommand cmd = db.CreateCommand(q))
            {
                TestContext.WriteLine(q.ToSQLString(db));
                if (cmd.Parameters.Count > 0)
                {
                    foreach (DbParameter p in cmd.Parameters)
                    {
                        TestContext.WriteLine("Parameter: {0}", p);
                    }
                }
            }
            
        }

    }
}
