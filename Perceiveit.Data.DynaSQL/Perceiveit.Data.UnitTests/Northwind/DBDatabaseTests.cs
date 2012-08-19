using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.UnitTests.Northwind
{
    /// <summary>
    /// Unit test class to validate each of the Execute... Read, ReadOne, ReadEach, NonQuery and Scalar methods against a known table.
    /// </summary>
    [TestClass]
    public class DBDatabaseTests
    {

        /* SQL Server Connections                                        *
         * Change as required to point to a valid database               *
         * This connection and user will need to create a database table */
        public const string DbConnection = @"Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True;Pooling=False";
        public const string DbProvider = "System.Data.SqlClient";
        /* End SQL Server connections */


        /* Auto initialize to the database connection */
        public static DBDatabase _db = DBDatabase.Create(DbConnection, DbProvider);
        public static int _rowcount;

        /* Table and Field names */
        private const string TableName = "DSQL_KVPairs";
        private const string IdColumn = "id";
        private const string NameColumn = "name";

        /// <summary>
        /// Class to hold each KeyValuePair instance
        /// </summary>
        private class KVPair
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

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

        //
        // Testing init and teardown
        //

        #region public static void InitTable()

        /// <summary>
        /// Creates the required table and inserts the rows.
        /// </summary>
        [ClassInitialize()]
        public static void InitTable(TestContext context)
        {
            try
            {
                DBQuery drop = DBQuery.Drop.Table(TableName).IfExists();
                _db.ExecuteNonQuery(drop);
            }
            catch
            {
                //Do nothing
            }

            try
            {
                

                DBQuery create = DBQuery.Create.Table(TableName)
                                               .Add(IdColumn, System.Data.DbType.Int32, DBColumnFlags.PrimaryKey)
                                               .Add(NameColumn, System.Data.DbType.String, 50);
                _db.ExecuteNonQuery(create);
                _db.ExecuteNonQuery(DBQuery.InsertInto(TableName).Values((DBConst)1, (DBConst)"First"));
                _db.ExecuteNonQuery(DBQuery.InsertInto(TableName).Values((DBConst)2, (DBConst)"Second"));
                _db.ExecuteNonQuery(DBQuery.InsertInto(TableName).Values((DBConst)3, (DBConst)"Third"));
                _db.ExecuteNonQuery(DBQuery.InsertInto(TableName).Values((DBConst)4, (DBConst)"Fourth"));
                //Set the row count
                _rowcount = 4;
                context.WriteLine("Created database table");

            }
            catch (Exception ex)
            {
                throw new Exception("Could not initialize testing table : " + ex.Message, ex);
            }

        }

        #endregion

        #region public void TearDownTable()

        /// <summary>
        /// Cleans up the database by dropping the testing table
        /// </summary>
        [ClassCleanup()]
        public static void TearDownTable()
        {
            try
            {
                DBQuery drop = DBQuery.Drop.Table(TableName);
                _db.ExecuteNonQuery(drop);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not clean up testing table : " + ex.Message, ex);
            }
        }

        #endregion

        //
        // unit tests
        //

        string sqlall = "SELECT * FROM [" + TableName + "]";
        string sqlone = "SELECT * FROM [" + TableName + "] WHERE [" + IdColumn + "] = 1";
        string sqlall_error = "SELECT * FROM [" + TableName + "_noexist]";
        string sqlone_error = "SELECT * FROM [" + TableName + "_noexist] WHERE [" + IdColumn + "] = 1";
        string sqlone_noresult = "SELECT * FROM [" + TableName + "] WHERE [" + IdColumn + "] = 100";

        #region ExecuteRead with sql text

        //
        // ExecuteRead(string, DBEmptyCallback) 
        //
        
        [TestMethod()]
        public void DBDatabase_TestReadString_LocalList()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(sqlall, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_LocalVaraible()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnList()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnInstance()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachLocal()
        {
            List<KVPair> all = new List<KVPair>();
            
            //called with record
            _db.ExecuteReadEach(sqlall, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneLocal()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachReturn()
        {
            
            //called with record
            object[] all =_db.ExecuteReadEach(sqlall, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneReturn()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachEmpty()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneEmpty()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        
        //
        // with error handling but still valid
        //
        
        //
        // ExecuteRead(string, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_LocalList_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(sqlall, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_LocalVaraible_errorhandling()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnList_errorhandling()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnInstance_errorhandling()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachLocal_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //called with record
            _db.ExecuteReadEach(sqlall, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneLocal_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachReturn_errorhandling()
        {

            //called with record
            object[] all = _db.ExecuteReadEach(sqlall, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneReturn_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachEmpty_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneEmpty_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }


        //
        // with error handling INVALID
        //

        //
        // ExecuteRead(string, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_LocalList_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //read all into an exiting variable
            _db.ExecuteRead(sqlall_error, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(string, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_LocalVaraible_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone_error, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnList_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall_error, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteRead(string, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_ReturnInstance_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone_error, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachLocal_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //called with record
            _db.ExecuteReadEach(sqlall_error, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneLocal_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_error, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachReturn_errorcapture()
        {
            bool caught = false;
            //called with record
            object[] all = _db.ExecuteReadEach(sqlall_error, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneReturn_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone_error, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_EachEmpty_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall_error, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_OneEmpty_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_error, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // No Results
        //

        [TestMethod()]
        public void DBDatabase_TestReadOneString_ReturnNoResults()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEachString_ReturnNoResults()
        {
            //read one into a local variable
            object[] all = _db.ExecuteReadEach(sqlone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Length);
        }

        [TestMethod()]
        public void DBDatabase_TestReadOneString_NoResults()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_noresult, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEachString_NoResults()
        {
            List<KVPair> all = new List<KVPair>();

            //read one into a local variable
            _db.ExecuteReadEach(sqlone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Count);
        }




        #endregion

        #region ExecuteRead with sql text and command type

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalList()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(sqlall, CommandType.Text, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalVaraible()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone, CommandType.Text, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnList()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall, CommandType.Text, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnInstance()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone, CommandType.Text, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachLocal()
        {
            List<KVPair> all = new List<KVPair>();

            //called with record
            _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneLocal()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachReturn()
        {

            //called with record
            object[] all = _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneReturn()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachEmpty()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneEmpty()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }


        //
        // with error handling but still valid
        //

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalList_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(sqlall, CommandType.Text, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalVaraible_errorhandling()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone, CommandType.Text, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnList_errorhandling()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall, CommandType.Text, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnInstance_errorhandling()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone, CommandType.Text, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachLocal_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //called with record
            _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneLocal_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachReturn_errorhandling()
        {

            //called with record
            object[] all = _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneReturn_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachEmpty_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall, CommandType.Text, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneEmpty_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }


        //
        // with error handling INVALID
        //

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalList_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //read all into an exiting variable
            _db.ExecuteRead(sqlall_error, CommandType.Text, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(string, CommandType, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_LocalVaraible_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(sqlone_error, CommandType.Text, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnList_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(sqlall_error, CommandType.Text, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteRead(string, CommandType, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_ReturnInstance_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(sqlone_error, CommandType.Text, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachLocal_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //called with record
            _db.ExecuteReadEach(sqlall_error, CommandType.Text, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, CommandType, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneLocal_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_error, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachReturn_errorcapture()
        {
            bool caught = false;
            //called with record
            object[] all = _db.ExecuteReadEach(sqlall_error, CommandType.Text, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneReturn_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone_error, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(string, CommandType, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_EachEmpty_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(sqlall_error, CommandType.Text, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(string, CommandType, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadString_CommandType_OneEmpty_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_error, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // No Results
        //

        [TestMethod()]
        public void DBDatabase_TestReadOneString_CommandType_ReturnNoResults()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(sqlone_noresult, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEachString_CommandType_ReturnNoResults()
        {
            //read one into a local variable
            object[] all = _db.ExecuteReadEach(sqlone_noresult, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Length);
        }

        [TestMethod()]
        public void DBDatabase_TestReadOneString_CommandType_NoResults()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(sqlone_noresult, CommandType.Text, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEachString_CommandType_NoResults()
        {
            List<KVPair> all = new List<KVPair>();

            //read one into a local variable
            _db.ExecuteReadEach(sqlone_noresult, CommandType.Text, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Count);
        }




        #endregion

        #region ExecuteRead with DbCommand

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_LocalList()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //read all into an exiting variable
                _db.ExecuteRead(cmd, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                });
            }
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_LocalVaraible()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into an existing variable
                _db.ExecuteRead(cmd, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;
                });
            }
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnList()
        {
            List<KVPair> all = null;
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(cmd, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnInstance()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(cmd, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachLocal()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {

                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);
                });

            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneLocal()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {

                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachReturn()
        {

            //called with record
            object[] all;
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                all = _db.ExecuteReadEach(cmd, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                });
            }

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadDbCommand_OneReturn()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachEmpty()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneEmpty()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }


        //
        // with error handling but still valid
        //

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestReadDbCommand_LocalList_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {

                //read all into an exiting variable
                _db.ExecuteRead(cmd, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_LocalVaraible_errorhandling()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into an existing variable
                _db.ExecuteRead(cmd, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnList_errorhandling()
        {
            List<KVPair> all = null;

            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(cmd, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnInstance_errorhandling()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(cmd, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachLocal_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();
            
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneLocal_errorhandling()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachReturn_errorhandling()
        {
            object[] all;
            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //called with record
                all = _db.ExecuteReadEach(cmd, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneReturn_errorhandling()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachEmpty_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbCommand cmd = _db.CreateCommand(sqlall))
            {
                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneEmpty_errorhandling()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone))
            {

                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }


        //
        // with error handling INVALID sql. 
        // Should be handled in onerror block
        //

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_LocalList_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            using (DbCommand cmd = _db.CreateCommand(sqlall_error))
            {

                //read all into an exiting variable
                _db.ExecuteRead(cmd, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(cmd, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_LocalVaraible_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone_error))
            {

                //read one into an existing variable
                _db.ExecuteRead(cmd, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
            Assert.IsNull(one);
        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnList_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = null;
            using (DbCommand cmd = _db.CreateCommand(sqlall_error))
            {
                //read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(cmd, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteRead(cmd, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_ReturnInstance_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone_error))
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(cmd, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachLocal_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            using (DbCommand cmd = _db.CreateCommand(sqlall_error))
            {
                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(cmd, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneLocal_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            using (DbCommand cmd = _db.CreateCommand(sqlone_error))
            {
                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachReturn_errorcapture()
        {
            bool caught = false;

            using (DbCommand cmd = _db.CreateCommand(sqlall_error))
            {
                //called with record
                object[] all = _db.ExecuteReadEach(cmd, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestReadDbCommand_OneReturn_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            using (DbCommand cmd = _db.CreateCommand(sqlone_error))
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }

            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(cmd, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_EachEmpty_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = new List<KVPair>();

            using (DbCommand cmd = _db.CreateCommand(sqlall_error))
            {
                //called with record
                _db.ExecuteReadEach(cmd, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(cmd, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DbCommand_OneEmpty_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone_error))
            {
                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // No Results
        //

        [TestMethod()]
        public void DBDatabase_TestReadOne_DbCommand_ReturnNoResults()
        {
            KVPair one = null;

            using (DbCommand cmd = _db.CreateCommand(sqlone_noresult))
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DbCommand_ReturnNoResults()
        {
            //read one into a local variable
            object[] all;


            using (DbCommand cmd = _db.CreateCommand(sqlone_noresult))
            {
                all = _db.ExecuteReadEach(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }

            Assert.AreEqual(0, all.Length);
        }

        [TestMethod()]
        public void DBDatabase_TestReadOne_DbCommand_NoResults()
        {
            KVPair one = null;
            using (DbCommand cmd = _db.CreateCommand(sqlone_noresult))
            {
                //read one into a local variable
                _db.ExecuteReadOne(cmd, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DbCommand_NoResults()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbCommand cmd = _db.CreateCommand(sqlone_noresult))
            {
                //read one into a local variable
                _db.ExecuteReadEach(cmd, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.AreEqual(0, all.Count);
        }




        #endregion

        DBQuery qall = DBQuery.SelectAll().From(TableName);
        DBQuery qone = DBQuery.SelectAll().From(TableName).WhereField(IdColumn, Compare.Equals, (DBConst)1);
        DBQuery qall_error = DBQuery.SelectAll().From(TableName + "_noexist");
        DBQuery qone_error = DBQuery.SelectAll().From(TableName + "_noexist").WhereField(IdColumn, Compare.Equals, (DBConst)1);
        DBQuery qone_noresult = DBQuery.SelectAll().From(TableName).WhereField(IdColumn, Compare.Equals, (DBConst)100);


        #region ExecuteRead with DBQuery

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalList()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(qall, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalVaraible()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(qone, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnList()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(qall, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnInstance()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(qone, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachLocal()
        {
            List<KVPair> all = new List<KVPair>();

            //called with record
            _db.ExecuteReadEach(qall, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneLocal()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachReturn()
        {

            //called with record
            object[] all = _db.ExecuteReadEach(qall, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneReturn()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(qone, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachEmpty()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(qall, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneEmpty()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });
            Assert.IsNotNull(one);
        }


        //
        // with error handling but still valid
        //

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalList_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //read all into an exiting variable
            _db.ExecuteRead(qall, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalVaraible_errorhandling()
        {
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(qone, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnList_errorhandling()
        {
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(qall, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnInstance_errorhandling()
        {
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(qone, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachLocal_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            //called with record
            _db.ExecuteReadEach(qall, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneLocal_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachReturn_errorhandling()
        {

            //called with record
            object[] all = _db.ExecuteReadEach(qall, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneReturn_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(qone, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachEmpty_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(qall, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneEmpty_errorhandling()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                throw new ArgumentException("Should not error");
            });
            Assert.IsNotNull(one);
        }


        //
        // with error handling INVALID
        //

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalList_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //read all into an exiting variable
            _db.ExecuteRead(qall_error, reader =>
            {
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    all.Add(kvp);
                }
            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_LocalVaraible_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into an existing variable
            _db.ExecuteRead(qone_error, reader =>
            {
                if (reader.Read())
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                else
                    one = null;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnList_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = null;
            //read all into a local variable and return that
            all = (List<KVPair>)_db.ExecuteRead(qall_error, reader =>
            {
                List<KVPair> inner = new List<KVPair>();
                while (reader.Read())
                {
                    KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    inner.Add(kvp);
                }
                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteRead(DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_ReturnInstance_errorcapture()
        {
            bool caught = false;
            KVPair one = null;
            //read one into a local variable and retrun it
            one = (KVPair)_db.ExecuteRead(qone_error, reader =>
            {
                KVPair inner = null;
                if (reader.Read())
                    inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                return inner;

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachLocal_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            //called with record
            _db.ExecuteReadEach(qall_error, record =>
            {
                KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                all.Add(kvp);

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneLocal_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            _db.ExecuteReadOne(qone_error, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachReturn_errorcapture()
        {
            bool caught = false;
            //called with record
            object[] all = _db.ExecuteReadEach(qall_error, record =>
            {
                return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneReturn_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(qone_error, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_EachEmpty_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = new List<KVPair>();
            //called with record
            _db.ExecuteReadEach(qall_error, record =>
            {
                all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBQuery_OneEmpty_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone_error, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

            }, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });
            Assert.IsTrue(caught);
        }

        //
        // No Results
        //

        [TestMethod()]
        public void DBDatabase_TestReadOne_DBQuery_ReturnNoResults()
        {
            KVPair one = null;

            //read one into a local variable
            one = (KVPair)_db.ExecuteReadOne(qone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DBQuery_ReturnNoResults()
        {
            //read one into a local variable
            object[] all = _db.ExecuteReadEach(qone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Length);
        }

        [TestMethod()]
        public void DBDatabase_TestReadOne_DBQuery_NoResults()
        {
            KVPair one = null;

            //read one into a local variable
            _db.ExecuteReadOne(qone_noresult, reader =>
            {
                one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DBQuery_NoResults()
        {
            List<KVPair> all = new List<KVPair>();

            //read one into a local variable
            _db.ExecuteReadEach(qone_noresult, reader =>
            {
                return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
            });

            Assert.AreEqual(0, all.Count);
        }




        #endregion

        #region ExecuteRead with DBTransaction and DBQuery

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalList()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbTransaction t = _db.BeginTransaction())
            {
                //read all into an exiting variable
                _db.ExecuteRead(t, qall, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                });
            }
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalVaraible()
        {
            KVPair one = null;
            using (DbTransaction t = _db.BeginTransaction())
            {//read one into an existing variable
                _db.ExecuteRead(t, qone, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;
                });
            }
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnList()
        {
            List<KVPair> all = null;
            using (DbTransaction t = _db.BeginTransaction())
            {//read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(t, qall, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnInstance()
        {
            KVPair one = null;
            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(t, qone, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachLocal()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                _db.ExecuteReadEach(t, qall, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneLocal()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachReturn()
        {
            object[] all;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                all = _db.ExecuteReadEach(t, qall, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                });
            }

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneReturn()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(t, qone, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachEmpty()
        {
            List<KVPair> all = new List<KVPair>();
            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                _db.ExecuteReadEach(t, qall, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneEmpty()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNotNull(one);
        }


        //
        // with error handling but still valid
        //

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalList_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read all into an exiting variable
                _db.ExecuteRead(t, qall, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalVaraible_errorhandling()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into an existing variable
                _db.ExecuteRead(t, qone, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnList_errorhandling()
        {
            List<KVPair> all = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(t, qall, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnInstance_errorhandling()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(t, qone, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachLocal_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                _db.ExecuteReadEach(t, qall, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneLocal_errorhandling()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachReturn_errorhandling()
        {
            object[] all;
            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                all = _db.ExecuteReadEach(t, qall, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Length);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneReturn_errorhandling()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(t, qone, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachEmpty_errorhandling()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                _db.ExecuteReadEach(t, qall, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }

            Assert.IsNotNull(all);
            Assert.AreEqual(_rowcount, all.Count);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneEmpty_errorhandling()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    throw new ArgumentException("Should not error");
                });
            }
            Assert.IsNotNull(one);
        }


        //
        // with error handling INVALID
        //

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalList_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read all into an exiting variable
                _db.ExecuteRead(t, qall_error, reader =>
                {
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        all.Add(kvp);
                    }
                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBEmptyCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_LocalVaraible_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into an existing variable
                _db.ExecuteRead(t, qone_error, reader =>
                {
                    if (reader.Read())
                        one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    else
                        one = null;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnList_errorcapture()
        {
            bool caught = false;
            List<KVPair> all = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read all into a local variable and return that
                all = (List<KVPair>)_db.ExecuteRead(qall_error, reader =>
                {
                    List<KVPair> inner = new List<KVPair>();
                    while (reader.Read())
                    {
                        KVPair kvp = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        inner.Add(kvp);
                    }
                    return inner;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteRead(DbTransacton, DBQuery, DBCallback) 
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_ReturnInstance_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable and retrun it
                one = (KVPair)_db.ExecuteRead(t, qone_error, reader =>
                {
                    KVPair inner = null;
                    if (reader.Read())
                        inner = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                    return inner;

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachLocal_errorcapture()
        {
            List<KVPair> all = new List<KVPair>();
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                _db.ExecuteReadEach(t, qall_error, record =>
                {
                    KVPair kvp = new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };
                    all.Add(kvp);

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBEmptyCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneLocal_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone_error, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachReturn_errorcapture()
        {
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //called with record
                object[] all = _db.ExecuteReadEach(t, qall_error, record =>
                {
                    return new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneReturn_errorcapture()
        {
            KVPair one = null;
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(t, qone_error, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // ExecuteReadEach(DbTransacton, DBQuery, DBEmptyRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_EachEmpty_errorcapture()
        {
            bool caught = false;

            using (DbTransaction t = _db.BeginTransaction())
            {
                List<KVPair> all = new List<KVPair>();
                //called with record
                _db.ExecuteReadEach(t, qall_error, record =>
                {
                    all.Add(new KVPair() { Id = record.GetInt32(0), Name = record.GetString(1) });

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);

        }

        //
        // ExecuteReadOne(DbTransacton, DBQuery, DBRecordCallback)
        //

        [TestMethod()]
        public void DBDatabase_TestRead_DBTransactionDBQuery_OneEmpty_errorcapture()
        {
            bool caught = false;
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone_error, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };

                }, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });
            }
            Assert.IsTrue(caught);
        }

        //
        // No Results
        //

        [TestMethod()]
        public void DBDatabase_TestReadOne_DBTransactionDBQuery_ReturnNoResults()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                one = (KVPair)_db.ExecuteReadOne(t, qone_noresult, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DBTransactionDBQuery_ReturnNoResults()
        {
            //read one into a local variable
            object[] all;

            using (DbTransaction t = _db.BeginTransaction())
            {

                all = _db.ExecuteReadEach(t, qone_noresult, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.AreEqual(0, all.Length);
        }

        [TestMethod()]
        public void DBDatabase_TestReadOne_DBTransactionDBQuery_NoResults()
        {
            KVPair one = null;

            using (DbTransaction t = _db.BeginTransaction())
            {
                //read one into a local variable
                _db.ExecuteReadOne(t, qone_noresult, reader =>
                {
                    one = new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.IsNull(one);
        }


        [TestMethod()]
        public void DBDatabase_TestReadEach_DBTransactionDBQuery_NoResults()
        {
            List<KVPair> all = new List<KVPair>();

            using (DbTransaction t = _db.BeginTransaction())
            {

                //read one into a local variable
                _db.ExecuteReadEach(t, qone_noresult, reader =>
                {
                    return new KVPair() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                });
            }
            Assert.AreEqual(0, all.Count);
        }




        #endregion


        string sqlins = "INSERT INTO [" + TableName + "] ([" + IdColumn + "],[" + NameColumn + "]) VALUES ({0},'{1}')";
        string sqlins_error = "INSERT INTO [" + TableName + "_noexist] ([" + IdColumn + "],[" + NameColumn + "]) VALUES ({0},'{1}')";


        //Being a bit clever here and directly returning the _rowcount value in the parameter evaluation.
        //This will be looked up everytime the query is executed (not during initialization).

        DBQuery qins = DBQuery.InsertInto(TableName)
                              .Fields(IdColumn, NameColumn)
                              .Values(DBParam.ParamWithDelegate(DbType.Int32, delegate{ return (_rowcount+1);}),
                                      DBParam.ParamWithDelegate(DbType.String, delegate { return "Item " + (_rowcount+1).ToString(); }));

        DBQuery qins_error = DBQuery.InsertInto(TableName + "_noexist")
                                    .Fields(IdColumn, NameColumn)
                                    .Values(DBParam.ParamWithDelegate(DbType.Int32, delegate { return (_rowcount+1); }),
                                            DBParam.ParamWithDelegate(DbType.String , delegate { return "Item " + (_rowcount+1).ToString(); }));


        #region ExecuteNonQuery with sql text

        [TestMethod()]
        public void DBDatabase_TestNonQuery_String_Insert()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount+1), "Item " + (_rowcount+1).ToString());

            int count = _db.ExecuteNonQuery(fullsql);
            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_String_Insert_errorhandling()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());

            int count = _db.ExecuteNonQuery(fullsql, onerror =>
            {
                throw new ArgumentException("Should not get here as no error", onerror.Exception);
            });

            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_String_Insert_errorcapture()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins_error, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());

            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            int count = _db.ExecuteNonQuery(fullsql, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
            //No increments as fails
        }
        
        #endregion

        #region ExecuteNonQuery with sql text and command type

        [TestMethod()]
        public void DBDatabase_TestNonQuery_StringCommandType_Insert()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());

            int count = _db.ExecuteNonQuery(fullsql, CommandType.Text);
            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_StringCommandType_Insert_errorhandling()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());

            int count = _db.ExecuteNonQuery(fullsql, CommandType.Text, onerror =>
            {
                throw new ArgumentException("Should not get here as no error", onerror.Exception);
            });

            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_StringCommandType_Insert_errorcapture()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins_error, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());

            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            int count = _db.ExecuteNonQuery(fullsql, CommandType.Text, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
            //No increments as fails
        }

        #endregion

        #region ExecuteNonQuery with DbCommand

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DbCommand_Insert()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());
            using (DbCommand cmd = _db.CreateCommand(fullsql))
            {
                int count = _db.ExecuteNonQuery(cmd);
                Assert.AreEqual(1, count);
                _rowcount += count;
            }
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DbCommand_Insert_errorhandling()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());
            using (DbCommand cmd = _db.CreateCommand(fullsql))
            {
                int count = _db.ExecuteNonQuery(cmd, onerror =>
                {
                    throw new ArgumentException("Should not get here as no error", onerror.Exception);
                });

                Assert.AreEqual(1, count);
                _rowcount += count;
            }
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DbCommand_Insert_errorcapture()
        {
            //Build the full SQL string
            string fullsql = string.Format(sqlins_error, (_rowcount + 1), "Item " + (_rowcount + 1).ToString());
            using (DbCommand cmd = _db.CreateCommand(fullsql))
            {
                bool caught = false;
                //Be carefull - cannot convert returned value to int as null will be returned
                int count = _db.ExecuteNonQuery(cmd, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });

                Assert.IsTrue(caught);
                //No increments as fails
            }
        }

        #endregion

        #region ExecuteNonQuery with DBQuery

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBQuery_Insert()
        {
            int count = _db.ExecuteNonQuery(qins);
            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBQuery_Insert_errorhandling()
        {
            //Nothing to update as methods get the query from 
            int count = _db.ExecuteNonQuery(qins, onerror =>
            {
                throw new ArgumentException("Should not get here as no error", onerror.Exception);
            });

            Assert.AreEqual(1, count);
            _rowcount += count;
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBQuery_Insert_errorcapture()
        {
            
            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            int count = _db.ExecuteNonQuery(qins_error, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
            //No increments as fails
        }

        #endregion

        #region ExecuteNonQuery with DBTransaction and DBQuery

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBTansactionDBQuery_Insert()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                int count = _db.ExecuteNonQuery(t, qins);
                Assert.AreEqual(1, count);
                t.Commit();

                _rowcount += count;
            }
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBTansactionDBQuery_Insert_errorhandling()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                int count = _db.ExecuteNonQuery(t, qins, onerror =>
                {
                    throw new ArgumentException("Should not get here as no error", onerror.Exception);
                });
                t.Commit();
                Assert.AreEqual(1, count);
                _rowcount += count;
            }
        }

        [TestMethod()]
        public void DBDatabase_TestNonQuery_DBTansactionDBQuery_Insert_errorcapture()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                bool caught = false;
                //Be carefull - cannot convert returned value to int as null will be returned
                int count = _db.ExecuteNonQuery(t, qins_error, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });

                Assert.IsTrue(caught);
                //No increments as fails
            }
        }


        #endregion


        string sqlcount = "SELECT COUNT(*) FROM [" + TableName + "]";
        string sqlcount_error = "SELECT COUNT(*) FROM [" + TableName + "_noexist]";
        DBQuery qcount = DBQuery.SelectCount().From(TableName);
        DBQuery qcount_error = DBQuery.SelectCount().From(TableName + "_noexist");

        #region ExecuteScalar with sql text

        [TestMethod()]
        public void DBDatabase_TestScalar_String_Count()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(sqlcount));
            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_String_Count_errorhandling()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(sqlcount, onerror =>
            {
                throw new ArgumentException("Should not get here as no error");
            }));

            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_String_Count_errorcapture()
        {
            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            object value = _db.ExecuteScalar(sqlcount_error, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
        }

        #endregion

        #region ExecuteScalar with sql text and command type

        [TestMethod()]
        public void DBDatabase_TestScalar_String_CommandType_Count()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(sqlcount, CommandType.Text));
            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_String_CommandType_Count_errorhandling()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(sqlcount, CommandType.Text, onerror =>
            {
                throw new ArgumentException("Should not get here as no error");
            }));

            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_String_CommandType_Count_errorcapture()
        {
            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            object value = _db.ExecuteScalar(sqlcount_error, CommandType.Text, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
        }

        #endregion

        #region ExecuteScalar with DbCommand

        [TestMethod()]
        public void DBDatabase_TestScalar_DBCommand_Count()
        {
            using (DbCommand cmd = _db.CreateCommand(sqlcount))
            {
                int count = Convert.ToInt32(_db.ExecuteScalar(cmd));
                Assert.AreEqual(_rowcount, count);
            }
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBCommand_Count_errorhandling()
        {
            using (DbCommand cmd = _db.CreateCommand(sqlcount))
            {
                int count = Convert.ToInt32(_db.ExecuteScalar(cmd, onerror =>
                {
                    throw new ArgumentException("Should not get here as no error");
                }));

                Assert.AreEqual(_rowcount, count);
            }
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBCommand_Count_errorcapture()
        {
            using (DbCommand cmd = _db.CreateCommand(sqlcount_error))
            {
                bool caught = false;
                //Be carefull - cannot convert returned value to int as null will be returned
                object value = _db.ExecuteScalar(cmd, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });

                Assert.IsTrue(caught);
            }
        }

        #endregion

        #region ExecuteScalar with DBQuery

        [TestMethod()]
        public void DBDatabase_TestScalar_DBQuery_Count()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(qcount));
            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBQuery_Count_errorhandling()
        {
            int count = Convert.ToInt32(_db.ExecuteScalar(qcount, onerror =>
            {
                throw new ArgumentException("Should not get here as no error");
            }));

            Assert.AreEqual(_rowcount, count);
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBQuery_Count_errorcapture()
        {
            bool caught = false;
            //Be carefull - cannot convert returned value to int as null will be returned
            object value = _db.ExecuteScalar(qcount_error, onerror =>
            {
                onerror.Handled = true;
                caught = true;
            });

            Assert.IsTrue(caught);
        }

        #endregion

        #region ExecuteScalar with DBTransaction and DBQuery

        [TestMethod()]
        public void DBDatabase_TestScalar_DBTransactionDBQuery_Count()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                int count = Convert.ToInt32(_db.ExecuteScalar(t, qcount));
                Assert.AreEqual(_rowcount, count);
            }
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBTransactionDBQuery_Count_errorhandling()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                int count = Convert.ToInt32(_db.ExecuteScalar(t, qcount, onerror =>
                {
                    throw new ArgumentException("Should not get here as no error");
                }));

                Assert.AreEqual(_rowcount, count);
            }
        }

        [TestMethod()]
        public void DBDatabase_TestScalar_DBTransactionDBQuery_Count_errorcapture()
        {
            using (DbTransaction t = _db.BeginTransaction())
            {
                bool caught = false;
                //Be carefull - cannot convert returned value to int as null will be returned
                object value = _db.ExecuteScalar(t, qcount_error, onerror =>
                {
                    onerror.Handled = true;
                    caught = true;
                });

                Assert.IsTrue(caught);
            }
        }

        #endregion

        }

    
}
