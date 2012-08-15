using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Perceiveit.Data.Query;
using System.Data;

namespace Perceiveit.Data.DynaSql.Tests
{
    [TestClass]
    public class DynaSQLCreateTests
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

        /* SQL Server Connections */
        public const string DbConnection = @"Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True;Pooling=False";
        public const string DbProvider = "System.Data.SqlClient";
        /* End SQL Server connections */

        /* MySql Connections 
        public const string DbConnection = "server=172.16.56.1;User Id=testaccount;Password=test;Persist Security Info=True;database=northwind;AllowUserVariables=true";
        public const string DbProvider = "MySql.Data.MySqlClient";
        /* End SQL Server connections */

        /* SQLite connections 
        public const string DbConnection = @"data source=C:\Development\DynaSQL\DynaSQLUnitTest.db;foreign keys=true;"; //we add the foreign keys=true here to enforce them
        public const string DbProvider = "System.Data.SQLite";
        /* End SQLite connections */

        /* MS Access connections 
        public const string DbConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Sample Databases\Northwind2007.accdb;Persist Security Info=False;";
        public const string DbProvider = "System.Data.OleDb";
        /* End MSAccess connections */

        #region private static DBDatabase CreateDb()

        /// <summary>
        /// Creates the database connection
        /// </summary>
        /// <returns></returns>
        private static DBDatabase ConnectDb()
        {
            return DBDatabase.Create(DbConnection, DbProvider);
        }

        #endregion

        const string TableName = "DSQL_CustomTable";
        const string TblCol1 = "ColA";
        const string TblCol2 = "ColB";
        const string TblCol3 = "ColC";

        const string ViewName = "DSQL_CustomView";

        [TestMethod()]
        public void _00_EnumerateProviders()
        {
            System.Data.DataTable table = System.Data.Common.DbProviderFactories.GetFactoryClasses();
            foreach (System.Data.DataRow row in table.Rows)
            {
                TestContext.WriteLine("{0}, {1}, {2}, {3}", row[0], row[1], row[2], row[3]);
                System.Data.Common.DbProviderFactory fact = System.Data.Common.DbProviderFactories.GetFactory(row);
            }
        }

        #region public void _01_JustCreateATable()

        /// <summary>
        /// Just creates the custom table
        /// </summary>
        [TestMethod()]
        public void _01_JustCreateATable()
        {
            DBDatabase db = ConnectDb();
            CreateCustomTable(db);
        }

        #endregion

        [TestMethod()]
        public void _02_JustCreateTheView()
        {
            DBDatabase db = ConnectDb();
            DBQuery create = GetCustomViewQuery();
            TestContext.WriteLine(create.ToSQLString(db));
            db.ExecuteNonQuery(create);
        }
       
        [TestMethod()]
        public void _03_JustDropTheView()
        {
            DBDatabase db = ConnectDb();

            DBQuery drop = DBQuery.Drop.View(ViewName);
            TestContext.WriteLine(drop.ToSQLString(db));

            db.ExecuteNonQuery(drop);
        }


        [TestMethod()]
        public void _04_JustDropTheTable()
        {
            DBDatabase db = ConnectDb();

            this.DropCustomTable(db);
        }

        /// <summary>
        /// Creates a custom table with the TableName,
        /// inserts a row, checks the row, 
        /// drops the table and vaildates it has dropped.
        /// </summary>
        [TestMethod]
        public void _05_CreateAndDropTable()
        {
            DBDatabase db = ConnectDb();
            

            //Create the table
            CreateCustomTable(db);


            //Insert a single row in the database
            DBQuery ins = InsertARowIntoCustomTable(db, "First", "Second");


            //Count the number of rows in the table
            DBQuery count = DBQuery.SelectCount().From(TableName);
            int result = Convert.ToInt32(db.ExecuteScalar(count));
            Assert.AreEqual(1, result);
            TestContext.WriteLine("The row was inserted and the the returned count was '{0}'", result);


            //Select all the rows and read
            DBQuery select = DBQuery.SelectAll().From(TableName);
            int readcount = 0;
            int colA;
            string colB = null;
            string colC = null;

            db.ExecuteRead(select, reader =>
            {
                while (reader.Read())
                {
                    colA = reader.GetInt32(0);
                    colB = reader.GetString(1);
                    colC = reader.GetString(2);
                    readcount++;
                }
            });
            //Based on the last row read (which should be the only row)
            Assert.AreEqual("First", colB);
            Assert.AreEqual("Second", colC);
            TestContext.WriteLine("Read {0} rows from the select statement and values were as predicted", readcount);


            //Dropping the table
            DropCustomTable(db);

            //Try another insert to make sure it fails
            try
            {
                InsertARowIntoCustomTable(db, "None", DBNull.Value);
                throw new InvalidOperationException("The table was not dropped and the insert statement succeeded");
            }
            catch (System.Exception ex)
            {
                TestContext.WriteLine("Expected failure after inserting on a dropped table:{0}", ex.Message);
            }
        }
        

        /// <summary>
        /// Creates a table and then a new view based on the custom table
        /// </summary>
        [TestMethod()]
        public void _06_CreateAndDropView()
        {
            DBDatabase db = ConnectDb();

            CreateCustomTable(db);

            //Wrap a try round this so we can definitely drop the table

            try
            {
                InsertARowIntoCustomTable(db, "Beta", "Alpha");
                InsertARowIntoCustomTable(db, "Alpha", "Beta");

                //Create the view
                DBQuery create = DBQuery.Create.View(ViewName)
                                        .As(DBQuery.SelectAll()
                                                   .From(TableName)
                                                   .WhereField("ColB", Compare.Like, DBConst.String("A%")));
                TestContext.WriteLine(create.ToSQLString(db));

                db.ExecuteNonQuery(create);
                TestContext.WriteLine("Created the view");

                DBQuery selectView = DBQuery.SelectAll().From(ViewName);
                int totalcount = 0;
                db.ExecuteRead(selectView, reader =>
                {
                    while (reader.Read())
                    {
                        string colb = reader["ColB"] as string;
                        totalcount += 1;
                    }
                });
                TestContext.WriteLine("Total Number of rows in view = {0}", totalcount);
                DBDropQuery drop = DBQuery.Drop.View(ViewName);
                db.ExecuteNonQuery(drop);

                //Try another select to make sure it fails
                try
                {
                    db.ExecuteNonQuery(selectView);
                    throw new InvalidOperationException("The table was not dropped and the insert statement succeeded");
                }
                catch(InvalidOperationException)
                {
                    throw;
                }
                catch (System.Exception ex)
                {
                    TestContext.WriteLine("Expected failure after inserting on a dropped table:{0}", ex.Message);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DropCustomTable(db);
            }
        }


        [TestMethod()]
        public void _07_DropTableIfExists()
        {
            DBDatabase db = ConnectDb();
            
            //Check for existance and drop
            DBQuery drop = DBQuery.Drop.Table(TableName).IfExists();

            TestContext.WriteLine(drop.ToSQLString(db));
            db.ExecuteNonQuery(drop);
            TestContext.WriteLine("Dropped table {0} if it existed\r\n", TableName);

            //Now create the table - if it still exists an error will be thrown
            CreateCustomTable(db);
            

            //Add one row
            InsertARowIntoCustomTable(db, "First", "Second");
            TestContext.WriteLine("Added 1 row to an existing table\r\n");

            //Get the number of rows (should be 1)
            DBQuery countSql = DBQuery.SelectCount().From(TableName);
            int count = Convert.ToInt32(db.ExecuteScalar(countSql));
            Assert.AreEqual(1, count);
            TestContext.WriteLine("Got the row count of {0} for table {1}\r\n", count, TableName);


            //Insert another row based on the table existing (which it should)
            DBParam p1 = DBParam.ParamWithValue(DbType.AnsiString, "Existing");
            DBParam p2 = DBParam.ParamWithValue(DbType.String, "Description");

            DBQuery insert = DBQuery.InsertInto(TableName).Fields("ColB", "ColC").Values(p1, p2);
            TestContext.WriteLine(insert.ToSQLString(db));
            db.ExecuteNonQuery(insert);
            
            //Re-Check the count and make sure one has been added
            int newcount = Convert.ToInt32(db.ExecuteScalar(countSql));
            Assert.AreEqual(count + 1, newcount);
            TestContext.WriteLine("Added 1 row to an existing table\r\n");

            //drop the table - checking for it first even though we know it exists
            db.ExecuteNonQuery(drop);
            TestContext.WriteLine("Checked existance and dropped " + TableName + " table\r\n");

            try
            {
                db.ExecuteNonQuery(DBQuery.InsertInto(TableName).Fields("ColB", "ColC").Values(p1, p2));
                throw new InvalidOperationException("The insert succeeded on a table that should have been dropped");
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                TestContext.WriteLine("Successfully caught exception for row insert");
            }
        }

        [TestMethod()]
        public void _08_CreateTableIfNotExists()
        {
            DBDatabase db = ConnectDb();

            //Make sure the table does not exist first
            DBQuery dropit = DBQuery.Drop.Table(TableName).IfExists();

            db.ExecuteNonQuery(dropit);

            DBParam p1 = DBParam.ParamWithValue(DbType.AnsiString, "Existing");
            DBParam p2 = DBParam.ParamWithValue(DbType.String, "Description");
            DBQuery insert = DBQuery.InsertInto(TableName).Fields("ColB", "ColC").Values(p1, p2);
            try
            {
                db.ExecuteNonQuery(insert);
                throw new InvalidOperationException("The insert succeeded on a table that should have been dropped");
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                TestContext.WriteLine("Successfully caught exception for row insert. We know the table does not exist");
            }

            //This should now not exist and be created
            DBQuery ifnotexistsCreate = GetCustomTableQuery().IfNotExists();
            TestContext.WriteLine(ifnotexistsCreate.ToSQLString(db));
            db.ExecuteNonQuery(ifnotexistsCreate);

            //Now we should be able to insert a row
            db.ExecuteNonQuery(insert);

            int count = Convert.ToInt32(db.ExecuteScalar(DBQuery.SelectCount().From(TableName)));
            //Check that the rows are there
            Assert.AreEqual(1, count);

            //Check that it does not try and create again
            db.ExecuteNonQuery(ifnotexistsCreate);

            //Validate that it has not just succeeded and re-created an empty table
            count = Convert.ToInt32(db.ExecuteScalar(DBQuery.SelectCount().From(TableName)));
            //Check that the rows are there
            Assert.AreEqual(1, count);

            //Finally just drop the table
            db.ExecuteNonQuery(dropit);


        }

        [TestMethod()]
        public void _09_CreateTableWithIndex()
        {
            DBDatabase db = ConnectDb();

            try
            {
                DBQuery tbl = DBQuery.Create.Table("CustomTable")
                                               .Add("col1", DbType.Int32, DBColumnFlags.PrimaryKey | DBColumnFlags.AutoAssign)
                                               .Add("col2", DbType.String, 50)
                                               .Add("col3", DbType.String, 255);

                //create 3 indexes on this table for the other columns
                DBQuery idx1 = DBQuery.Create.Index("CT_ColBoth").Unique().On("CustomTable")
                                             .Columns("col1", Order.Ascending, "col2", Order.Ascending);

                DBQuery idx2 = DBQuery.Create.Index(true, "CT_Col2").On("CustomTable")
                                             .Add("col2");

                DBQuery idx3 = DBQuery.Create.Index("CT_Col1").On("CustomTable")
                                             .Add("col1", Order.Ascending);

                if (db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts))
                {
                    DBScript create = DBQuery.Script(tbl, idx1, idx2, idx3);
                    TestContext.WriteLine(create.ToSQLString(db));
                    db.ExecuteNonQuery(create);
                }
                else //scripts are not supported so we are going to execute individually
                {
                    db.ExecuteNonQuery(tbl);
                    db.ExecuteNonQuery(idx1);
                    db.ExecuteNonQuery(idx2);
                    db.ExecuteNonQuery(idx3);
                }
            }
            finally
            {
                DBQuery droptbl = DBQuery.Drop.Table("CustomTable");
                DBQuery dropidx1 = DBQuery.Drop.Index("CT_ColBoth").On("CustomTable");
                DBQuery dropidx2 = DBQuery.Drop.Index("CT_Col2").On("CustomTable");
                DBQuery dropidx3 = DBQuery.Drop.Index("CT_Col1").On("CustomTable");

                if (db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts))
                {
                    DBScript dropall = DBScript.Script(dropidx3, dropidx2, dropidx1, droptbl);

                    TestContext.WriteLine("\r\n" + dropall.ToSQLString(db));

                    db.ExecuteNonQuery(dropall);
                }
                else
                {
                    db.ExecuteNonQuery(dropidx1);
                    db.ExecuteNonQuery(dropidx2);
                    db.ExecuteNonQuery(dropidx3);
                    db.ExecuteNonQuery(droptbl);
                }
            }
        }


        [TestMethod()]
        public void _10_CreateTableWithForeignKeys()
        {
            DBDatabase db = ConnectDb();

            //Create the persons table
            DBQuery createPersons = DBQuery.Create.Table("DSQL_Persons")
                                                  .Add("Person_ID", DbType.Int32, DBColumnFlags.AutoAssign | DBColumnFlags.PrimaryKey)
                                                  .Add("Person_Name", DbType.String, 50);

            TestContext.WriteLine(createPersons.ToSQLString(db));
            db.ExecuteNonQuery(createPersons);

            //Create the orders table
            DBQuery createOrders = DBQuery.Create.Table("DSQL_Orders")
                                                  .Add("Order_ID", DbType.Int32, DBColumnFlags.AutoAssign | DBColumnFlags.PrimaryKey)
                                                  .Add("Ordered_By", DbType.Int32)
                                                  .Add("Ordered_Date", DbType.DateTime)
                                                  .Add("Signed_By",DbType.Int32,DBColumnFlags.Nullable)
                                                  .Constraints(
                                                        DBConstraint.ForeignKey().Column("Ordered_By") //unnamed foreign key first
                                                                    .References("DSQL_Persons").Column("Person_ID"),

                                                        DBConstraint.ForeignKey("Orders_Signed_By_2_Persons_PersonID").Column("Signed_By")
                                                                    .References("DSQL_Persons").Column("Person_ID")
                                                                    .OnDelete(DBFKAction.Cascade)
                                                                    .OnUpdate(DBFKAction.Cascade)
                                                                );
            //Execute the Create Table statements
            TestContext.WriteLine(createOrders.ToSQLString(db));
            db.ExecuteNonQuery(createOrders);

            try
            {
                bool scripts = db.GetProperties().CheckSupports(DBSchemaTypes.CommandScripts);

                DBParam pname = DBParam.Param("name", DbType.String);

                DBScript insertperson = DBQuery.Script(
                                            DBQuery.InsertInto("DSQL_Persons").Field("Person_Name").Value(pname),
                                            DBQuery.Select(DBFunction.LastID())
                                            );

                //Insert one row into the persons table
                pname.Value = "First Person";
                TestContext.WriteLine(insertperson.ToSQLString(db));
                int firstpid = Convert.ToInt32(this.ExecuteScalarScript(db,insertperson,scripts));
                
                //And another row
                pname.Value = "Second Person";
                int secondpid = Convert.ToInt32(this.ExecuteScalarScript(db, insertperson, scripts));
                

                //Create an order with orderedby = firstpid and signedby = secondpid
                DBParam orderedby = DBParam.ParamWithValue(DbType.Int32, firstpid);
                DBParam signedby = DBParam.ParamWithValue(DbType.Int32, secondpid);
                DBScript insertorder = DBQuery.Script(
                                        DBQuery.InsertInto("DSQL_Orders")
                                                .Field("Ordered_By").Value(orderedby)
                                                .Field("Ordered_Date").Value(DBFunction.GetDate())
                                                .Field("Signed_By").Value(signedby),
                                        DBQuery.Select(DBFunction.LastID())
                                        );
                TestContext.WriteLine(insertorder.ToSQLString(db));
                int orderid = Convert.ToInt32(this.ExecuteScalarScript(db,insertorder,scripts));

                //Now try to create an order that breaks referential integrity
                orderedby.Value = -100;
                try
                {
                    orderid = Convert.ToInt32(db.ExecuteScalar(insertorder));
                    throw new InvalidOperationException("We should not be able to insert these rows. FAILED test");
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine("Sucessfully caught an exception that breaks referential integrity");
                }


                //Finally check the cascading deletes
                //check the Orders table count
                //delete a person row
                //ensure that the corresponding row for Signed By FK was deleted

                DBQuery getcount = DBQuery.SelectCount().From("DSQL_ORDERS");
                int ordercount = Convert.ToInt32(
                                    db.ExecuteScalar(getcount));

                DBQuery del = DBQuery.DeleteFrom("DSQL_Persons")
                                     .WhereField("Person_ID", Compare.Equals, DBParam.ParamWithValue(secondpid));
                int removed = db.ExecuteNonQuery(del);

                Assert.AreEqual(1, removed);
                TestContext.WriteLine("Removed a single row from the persons table");

                int newordercount = Convert.ToInt32(db.ExecuteScalar(getcount));
                //Make sure the orders row has been deleted
                Assert.AreEqual(newordercount, ordercount - 1);
                TestContext.WriteLine("Validated that the corresponding row in the orders table has been removed too");

            }
            finally
            {
                //Clean up tables in order
                db.ExecuteNonQuery(DBQuery.Drop.Table("DSQL_Orders"));
                db.ExecuteNonQuery(DBQuery.Drop.Table("DSQL_Persons"));
                TestContext.WriteLine("Database has been cleaned up");
            }
        }

        private object ExecuteScalarScript(DBDatabase db, DBScript script, bool supportsMultipleStatements)
        {
            if (supportsMultipleStatements)
                return db.ExecuteScalar(script);
            else
            {
                object value = null;
                using (System.Data.Common.DbConnection con = db.CreateConnection())
                {
                    con.Open();
                    foreach (DBQuery q in script)
                    {
                        value = db.ExecuteScalar(con, q);
                    }
                }
                return value;
            }
        }

        /// <summary>
        /// Attempts to drop thetables created for the foreign key tests
        /// </summary>
        [TestMethod()]
        public void _11_DropTheFKTables()
        {
            DBDatabase db = ConnectDb();
            try
            {
                db.ExecuteNonQuery(DBQuery.Drop.Table("DSQL_Persons"));
            }
            catch
            {
            }

            try
            {
                db.ExecuteNonQuery(DBQuery.Drop.Table("DSQL_Orders"));
            }
            catch
            {
            }

        }


        [TestMethod()]
        public void _12_CreateSProc()
        {
            DBDatabase db = ConnectDb();
            this.CreateCustomTable(db);
            try
            {
                DBParam name = DBParam.Param("name",DbType.String,50);
                DBParam desc = DBParam.Param("desc", DbType.String, 150);

                DBQuery ins = DBQuery.InsertInto(TableName).Fields(TblCol2, TblCol3).Values(name, desc);
                char one = 'A';
                char two = 'a';

                TestContext.WriteLine(ins.ToSQLString(db));

                for (int i = 0; i < 26; i++)
                {
                    int c = (int)one;
                    c += i;
                    string offset = ((char)c).ToString() + two.ToString();
                    name.Value = offset;
                    desc.Value = "Description of " + offset;
                    db.ExecuteNonQuery(ins);
                }
                int count = Convert.ToInt32(db.ExecuteScalar(DBQuery.SelectCount().From(TableName)));
                Assert.AreEqual(26, count);

                DBQuery q = DBQuery.Create.StoredProcedure("finditemsincol2")
                                          .WithParam("p1", DbType.String, 50, ParameterDirection.Input)
                                          .As(
                                                DBQuery.SelectAll().From(TableName)
                                                               .WhereField(TblCol2, Compare.Like, DBParam.Param("p1"))

                                             );
                TestContext.WriteLine("Execute Procedure: " + q.ToSQLString(db));
                db.ExecuteNonQuery(q);
                TestContext.WriteLine("Created the new stored procedure");


                DBQuery exec = DBQuery.Exec("finditemsincol2").WithParamValue("p1", DbType.String, 50, "A%");
                count = 0;
                TestContext.WriteLine(exec.ToSQLString(db));
                db.ExecuteRead(exec, reader =>
                {
                    while (reader.Read())
                    {
                        count++;
                        Assert.IsTrue(reader[TblCol2].ToString().StartsWith("A"));
                    }
                    TestContext.WriteLine("Executed the stored procedure and read '" + count.ToString() + "' rows");
                });

                Assert.AreEqual(1, count);
            }
            finally
            {
                try
                {
                    DBQuery drop = DBQuery.Drop.StoredProcedure("finditemsincol2");
                    db.ExecuteNonQuery(drop);
                    TestContext.WriteLine("Sucessfully dropped the stored procedure");
                }
                catch
                {
                    TestContext.WriteLine("DROP PROCEDURE failed");
                }
                this.DropCustomTable(db);
            }
        }


        [TestMethod()]
        public void _13_CreateSprocIfExists()
        {
        }


        //
        // support methods that actually perform the operation on the database
        //

        #region private void CreateCustomTable(DBDatabase db)

        /// <summary>
        /// Creates a new custom table in the specified database
        /// </summary>
        /// <param name="db"></param>
        private void CreateCustomTable(DBDatabase db)
        {
            DBCreateTableQuery create = GetCustomTableQuery();

            //OleDb provider does not support default values
            if (db.ProviderName == "System.Data.OleDb")
            {
                create.ClearDefaults();
            }

            TestContext.WriteLine(create.ToSQLString(db));
            db.ExecuteNonQuery(create);
            TestContext.WriteLine("Table {0} Created", TableName);
        }

        #endregion

        #region private static DBCreateTableQuery GetCustomTableQuery()

        /// <summary>
        /// returns the custom table creation statement
        /// </summary>
        /// <returns></returns>
        private static DBCreateTableQuery GetCustomTableQuery()
        {
            DBCreateTableQuery create = DBQuery.Create.Table(TableName)
                .Add(TblCol1, DbType.Int32, DBColumnFlags.AutoAssign | DBColumnFlags.PrimaryKey)
                .Add(TblCol2, DbType.AnsiString, 50, DBColumnFlags.Nullable).Default(DBConst.String("-"))
                .Add(TblCol3, DbType.String, 1000, DBColumnFlags.Nullable);
            return create;
        }

        #endregion

        #region private static DBSelectQuery GetViewSelect()

        /// <summary>
        /// Gets the inner select for the custom view
        /// </summary>
        /// <returns></returns>
        private static DBSelectQuery GetViewSelect()
        {
            return DBQuery.SelectAll()
                          .From(TableName)
                          .WhereField(TblCol2, Compare.Like, DBConst.String("A%"));
        }

        #endregion

        #region private static DBCreateViewQuery GetCustomViewQuery()

        /// <summary>
        /// returns the Statement to Create the custom view
        /// </summary>
        /// <returns></returns>
        private static DBCreateViewQuery GetCustomViewQuery()
        {
            DBSelectQuery sel = GetViewSelect();
            return DBQuery.Create.View(ViewName).As(sel);
        }

        #endregion

        #region private void DropCustomTable(DBDatabase db)

        /// <summary>
        /// Drops the custom table
        /// </summary>
        /// <param name="db"></param>
        private void DropCustomTable(DBDatabase db)
        {
            DBQuery drop = DBQuery.Drop.Table(TableName);
            TestContext.WriteLine(drop.ToSQLString(db));
            int droppedCount = db.ExecuteNonQuery(drop);
            TestContext.WriteLine("Table {0} dropped", TableName);
        }

        #endregion

        #region private DBQuery InsertARowIntoCustomTable(DBDatabase db, object first, object second)

        /// <summary>
        /// Returns a statement that will insert a row into the custom table - must have been created before
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DBQuery InsertARowIntoCustomTable(DBDatabase db, object first, object second)
        {
            DBParam p1 = DBParam.ParamWithValue(DbType.AnsiString, first);
            DBParam p2 = DBParam.ParamWithValue(DbType.String, second);

            DBQuery ins = DBQuery.InsertInto(TableName)
                .Fields(TblCol2, TblCol3)
                .Values(p1, p2);

            int count = db.ExecuteNonQuery(ins);
            
            return ins;
        }

        #endregion
    }
}
