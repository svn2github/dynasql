using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Perceiveit.Data.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Perceiveit.Data.UnitTests.SQLite
{
    [TestClass]
    public class SQLiteComplete
    {

        #region public DBDatabase Database {get;}

        private DBDatabase _database;

        /// <summary>
        /// Gets the database reference for these tests
        /// </summary>
        public DBDatabase Database
        {
            get { return _database; }
        }

        #endregion

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
        // Set up
        //

        #region public void InitDbs() + AttachTestContextProfiler()

        
        /// <summary>
        /// Sets up all the database connections that should be executed against
        /// </summary>
        [TestInitialize()]
        public void InitDbs()
        {
            //Modify the connections to suit
            //Comment any databases that should not be executed against

            DBDatabase sqlclient = DBDatabase.Create("SQLite"
                                                    , SQLite.Schools.DbConnection
                                                    , SQLite.Schools.DbProvider);
            AttachTestContextProfiler(sqlclient);
            this._database = sqlclient;
            this._database.HandleException += new DBExceptionHandler(database_HandleException);

            //DBDatabase mysql = DBDatabase.Create("mySql"
            //                                    , "server=172.16.56.1;User Id=testaccount;Password=test;Persist Security Info=True;database=northwind"
            //                                    , "MySql.Data.MySqlClient");
            //AttachTestContextProfiler(mysql);

            //DBDatabase sqlite = DBDatabase.Create("MSAccess"
            //                                    , @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Sample Databases\Northwind2007.accdb;Persist Security Info=False;"
            //                                    , "System.Data.OleDb");
            //AttachTestContextProfiler(sqlite);

            //DBDatabase oracle = DBDatabase.Create("Oracle"
            //                                     , "DATA SOURCE=127.0.0.1;PERSIST SECURITY INFO=True;USER ID=DBUSER;PASSWORD=Test"
            //                                     , "Oracle.DataAccess.Client");
            //AttachTestContextProfiler(oracle);


           
        }

        void database_HandleException(object sender, DBExceptionEventArgs args)
        {
            TestContext.WriteLine("Database encountered an error : {0}", args.Message);
            //Not nescessary - but hey, validates it's writable.
            args.Handled = false;
        }

        /// <summary>
        /// Attaches a profiler to the database connection.
        /// Reports the sql execution of all statements against the database to the TestContext
        /// </summary>
        /// <param name="database"></param>
        private void AttachTestContextProfiler(DBDatabase database)
        {
            TestContextProfiler profiler = new TestContextProfiler(database.Name, this.TestContext);
            database.AttachProfiler(profiler, true);
        }

        #endregion

        //
        // Test Method
        //

        #region public void SQLite_01_CreateV1Tables()

        /// <summary>
        /// Create all the Version 1 data tables using the SQLCLient Schools schema
        /// </summary>
        [TestMethod()]
        public void SQLite_01_CreateV1Tables()
        {
            DBDatabase db = this.Database;


            DBQuery create;

            //Create Departments
            create = DBQuery.Create.Table(Schools.Schema, Schools.Departments.Table)
                                           .Add(Schools.Departments.Id)
                                           .Add(Schools.Departments.Name)
                                           .Add(Schools.Departments.DateCreated)
                                           .Add(Schools.Departments.LastModified)
                                           .Add(Schools.Departments.Budget)
                                           .Add(Schools.Departments.Administrator);
            db.ExecuteNonQuery(create);

            //Create Courses
            create = DBQuery.Create.Table(Schools.Schema, Schools.Courses.Table)
                                    .Add(Schools.Courses.Id)
                                    .Add(Schools.Courses.Title)
                                    .Add(Schools.Courses.Credits)
                                    .Add(Schools.Courses.Department)
                                    .Add(Schools.Courses.Description)
                                    .Constraints(
                                        DBConstraint.ForeignKey().Column(Schools.Courses.Department)
                                                    .References(Schools.Departments.Table).Column(Schools.Departments.Id));
            db.ExecuteNonQuery(create);

            

            //Create Onsite Courses with an FK onto COURSES (cascade delete)
            create = DBQuery.Create.Table(Schools.Schema, Schools.OnsiteCourse.Table)
                                   .Add(Schools.OnsiteCourse.Id)
                                   .Add(Schools.OnsiteCourse.Location)
                                   .Add(Schools.OnsiteCourse.Days)
                                   .Add(Schools.OnsiteCourse.Time)
                                   .Constraints(
                                        DBConstraint.ForeignKey().Column(Schools.OnsiteCourse.Id)
                                                        .References(Schools.Courses.Table).Column(Schools.Courses.Id)
                                                        .OnDelete(DBFKAction.Cascade));
            db.ExecuteNonQuery(create);

            //Create Online Courses with a FK onto COURSES (cascade delete)
            create = DBQuery.Create.Table(Schools.Schema, Schools.OnlineCourse.Table)
                                   .Add(Schools.OnlineCourse.Id)
                                   .Add(Schools.OnlineCourse.Url)
                                   .Constraints(
                                        DBConstraint.ForeignKey().Column(Schools.OnlineCourse.Id)
                                                        .References(Schools.Courses.Table).Column(Schools.Courses.Id)
                                                        .OnDelete(DBFKAction.Cascade));
            db.ExecuteNonQuery(create);

            //Create Persons
            create = DBQuery.Create.Table(Schools.Schema, Schools.Person.Table)
                                   .Add(Schools.Person.Id)
                                   .Add(Schools.Person.First)
                                   .Add(Schools.Person.Last)
                                   .Add(Schools.Person.HireDate)
                                   .Add(Schools.Person.EnrollmentDate);
            db.ExecuteNonQuery(create);

            //Create Instructors
            create = DBQuery.Create.Table(Schools.Schema, Schools.Instructor.Table)
                                   .Add(Schools.Instructor.CourseID)
                                   .Add(Schools.Instructor.PersonID)
                                   .Constraints(
                                        DBConstraint.PrimaryKey().Columns(Schools.Instructor.CourseID, Schools.Instructor.PersonID),
                                        DBConstraint.ForeignKey().Column(Schools.Instructor.CourseID)
                                                        .References(Schools.Courses.Table).Column(Schools.Courses.Id),
                                        DBConstraint.ForeignKey().Column(Schools.Instructor.PersonID)
                                                        .References(Schools.Person.Table).Column(Schools.Person.Id));
            db.ExecuteNonQuery(create);

        }

        #endregion

        
        //
        // insert data from the SampleData files
        //

        #region public void SQLite_02_InsertDepartmentData()


        /// <summary>
        /// Loads all the Departments from a simple CSV file and inserts 
        /// them into the Departments data table using delegate parameters
        /// </summary>
        [TestMethod()]
        public void SQLite_02_InsertDepartmentData()
        {
            DBDatabase db = this.Database;

            //first check the original count on the database for comparison later
            DBQuery getcount = DBQuery.SelectCount().From(Schools.Schema, Schools.Departments.Table);
            int origcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            TestContext.WriteLine("Total number of departments before inserts = {0}", origcount);


            //Load Departments
            CSVData depts = CSVData.ParseString(SampleData.Departments, true);
            CSVItem[] all = depts.Items;
            CSVItem item = null;

            //Get the column offsets from the CSV file for name and budget - administrator comes later
            int nameoffset = depts.GetOffset("Name");
            int budgetoffset = depts.GetOffset("Budget");
            
            //Build parameters for retrieving the data at the offsets using anonymous delegates
            DBParam pname = DBParam.ParamWithDelegate(DbType.String, delegate { return item[nameoffset]; });
            DBParam pbudget = DBParam.ParamWithDelegate(DbType.Currency, delegate { return double.Parse(item[budgetoffset]); });
            

            //Insert query using fields onto parameters
            DBQuery ins = DBQuery.InsertInto(Schools.Schema, Schools.Departments.Table)
                                 .Fields(Schools.Departments.Name.Name, Schools.Departments.Budget.Name)
                                 .Values(pname, pbudget);


            //Wrap it all in a transaction
            using (DbTransaction trans = db.BeginTransaction())
            {
                int sum = 0;
                for (int i = 0; i < all.Length; i++)
                {
                    item = all[i];
                    sum += db.ExecuteNonQuery(trans, ins);
                }

                trans.Commit();
                TestContext.WriteLine("Number of inserts = {0}", sum);
            }


            //Validate that they have all been inserted
            int newcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            Assert.AreEqual(origcount + all.Length, newcount);
            TestContext.WriteLine("Total number of departments after inserts = {0}", newcount);
        }

        #endregion

        #region public void SQLite_03_InsertCourseData()

        [TestMethod()]
        public void SQLite_03_InsertCourseData()
        {

            DBDatabase db = this.Database;

            //first check the original count on the database for comparison later
            DBQuery getcount = DBQuery.SelectCount().From(Schools.Schema, Schools.Courses.Table);
            int origcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            TestContext.WriteLine("Total number of cousrses before inserts = {0}", origcount);


            //Load Departments
            CSVData course = CSVData.ParseString(SampleData.Courses, true);
            CSVItem[] all = course.Items;
            

            //Get the column offsets from the CSV file.
            int idoffset = course.GetOffset("ID");
            int titleoffset = course.GetOffset("Title");
            int credsoffset = course.GetOffset("Credits");
            int deptoffset = course.GetOffset("Department");
            

 
            //Different stategy here - do an insert into the courses based on a lookup against department
            //                         with the other parameters as values on that select

            DBParam pid = DBParam.ParamWithValue("id", Schools.Courses.Id.Type, null);
            DBParam ptitle = DBParam.ParamWithValue("title", Schools.Courses.Title.Type, Schools.Courses.Title.Length, null);
            DBParam pcreds = DBParam.ParamWithValue("creds", Schools.Courses.Credits.Type, null);

            DBParam pdept = DBParam.ParamWithValue("dept", DbType.String, null); //this is the name used to lookup the id - not the id itself


           
            DBQuery insert = DBQuery.InsertInto(Schools.Schema, Schools.Courses.Table)
                                    .Field(Schools.Courses.Id.Name)
                                    .Field(Schools.Courses.Title.Name)
                                    .Field(Schools.Courses.Credits.Name)
                                    .Field(Schools.Courses.Department.Name)
                                    .Select(
                                        DBQuery.Select(pid, ptitle, pcreds, DBField.Field(Schools.Departments.Id.Name))
                                                .From(Schools.Schema, Schools.Departments.Table)
                                                .WhereFieldEquals(Schools.Departments.Name.Name, pdept)
                                        );

            
            // Wrap it all in an exception again.
            using (DbTransaction trans = db.BeginTransaction())
            {
                int sum = 0;
                //now we update all the values on the parameters based on the current item
                foreach (CSVItem item in all)
                {
                    pid.Value = item[idoffset];
                    ptitle.Value = item[titleoffset];
                    pcreds.Value = Convert.ToSingle(item[credsoffset]);
                    pdept.Value = item[deptoffset];

                    //perform the execution
                    sum += Convert.ToInt32(db.ExecuteNonQuery(trans, insert));

                }
                trans.Commit();
                TestContext.WriteLine("Number of inserts = {0}", sum);
            }

            int newcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            Assert.AreEqual(origcount + all.Length, newcount);
            TestContext.WriteLine("Total number of courses after inserts = {0}", newcount);

        }

        #endregion

        #region public void SQLite_04_InsertOnSiteCourseData()

        /// <summary>
        /// Simple quick inserts using Const values for a change. 
        /// Performs a transactional rollback first and then re-does with a commit
        /// </summary>
        [TestMethod()]
        public void SQLite_04_InsertOnSiteCourseData()
        {
            DBDatabase db = this.Database;

            DBQuery getcount = DBQuery.SelectCount().From(Schools.Schema, Schools.OnsiteCourse.Table);
            int origcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            TestContext.WriteLine("Total number of onsite courses before inserts = {0}", origcount);

            //load the Onsite courses
            CSVData course = CSVData.ParseString(SampleData.OnsiteCourses, true);
            CSVItem[] all = course.Items;

            //Get the column offsets from the CSV file.
            int idoffset = course.GetOffset("ID");
            int locationoffset = course.GetOffset("Location");
            int daysoffset = course.GetOffset("Days");
            int timeoffset = course.GetOffset("Time");
            
            //create the constants with empty values
            DBConst id = DBConst.String("");
            DBConst loc = DBConst.String("");
            DBConst days = DBConst.Int32(0);
            DBConst time = DBConst.DateTime(DateTime.MinValue);

            DBQuery insert = DBQuery.InsertInto(Schools.Schema, Schools.OnsiteCourse.Table)
                                    .Fields(Schools.OnsiteCourse.Id.Name,
                                            Schools.OnsiteCourse.Location.Name,
                                            Schools.OnsiteCourse.Days.Name,
                                            Schools.OnsiteCourse.Time.Name)
                                    .Values(id, loc, days, time);

            using (DbTransaction trans = db.BeginTransaction())
            {
                foreach (CSVItem item in all)
                {
                    id.Value = item[idoffset];
                    loc.Value = item[locationoffset];
                    days.Value = Convert.ToInt32(item[daysoffset]);
                    time.Value = DateTime.ParseExact(item[timeoffset], "HH:mm:ss", null);

                    db.ExecuteNonQuery(trans, insert);
                }
                //Do not commit
            }
            int uncommitedCount = Convert.ToInt32(db.ExecuteScalar(getcount));
            Assert.AreEqual(origcount, uncommitedCount);
            TestContext.WriteLine("After uncommited inserts and transaction rollback the row count was still '{0}'", uncommitedCount);

            //repeat with a commit at the end
            using (DbTransaction trans = db.BeginTransaction())
            {
                foreach (CSVItem item in all)
                {
                    id.Value = item[idoffset];
                    loc.Value = item[locationoffset];
                    days.Value = Convert.ToInt32(item[daysoffset]);
                    time.Value = DateTime.ParseExact(item[timeoffset], "HH:mm:ss", null);

                    db.ExecuteNonQuery(trans, insert);
                }
                //Now commit
                trans.Commit();
            }

            int newCount = Convert.ToInt32(db.ExecuteScalar(getcount));
            Assert.AreEqual(origcount + all.Length, newCount);
            TestContext.WriteLine("After {0} committed inserts new count is {1}", all.Length, newCount);

        }

        #endregion

        #region public void SQLite_05_InsertOnlineCourseData()

        /// <summary>
        /// Non transactional inserts into the database for all the online courses
        /// </summary>
        [TestMethod()]
        public void SQLite_05_InsertOnlineCourseData()
        {
            DBDatabase db = this.Database;

            DBQuery getcount = DBQuery.SelectCount().From(Schools.Schema, Schools.OnlineCourse.Table);
            int origcount = Convert.ToInt32(db.ExecuteScalar(getcount));
            TestContext.WriteLine("Total number of online courses before inserts = {0}", origcount);

            CSVData data = CSVData.ParseString(SampleData.OnlineCourses, true);
            CSVItem[] all = data.Items;
            CSVItem item = null;

            int cidoffset = data.GetOffset("Id");
            int urloffset = data.GetOffset("Url");

            DBParam id = DBParam.ParamWithDelegate(delegate { return item[cidoffset]; });
            DBParam url = DBParam.ParamWithDelegate(delegate { return item[urloffset]; });

            //Test with the natural ordering of the columns
            DBQuery insert = DBQuery.InsertInto(Schools.Schema, Schools.OnlineCourse.Table)
                                    .Values(id, url);

            //Don't lock this in a transation - to ensure it is committed after each round.
            for (int i = 0; i < all.Length; i++)
            {
                item = all[i];
                db.ExecuteNonQuery(insert);

                //now check that it has been inserted.
                int count = Convert.ToInt32(db.ExecuteScalar(getcount));
                Assert.AreEqual(origcount + i + 1, count); //we add one because of the position in the loop
                TestContext.WriteLine("Inserted an online course and count is now {0}", count);
            }
        }

        #endregion

        #region public void SQLite_06_InsertPeopleData()

        /// <summary>
        /// Inserts all the people into the table with 
        /// NULL checking for the Hire and Enrollment dates
        /// </summary>
        [TestMethod()]
        public void SQLite_06_InsertPeopleData()
        {
            DBDatabase db = this.Database;

            CSVData data = CSVData.ParseString(SampleData.People, true);
            CSVItem[] all = data.Items;
            CSVItem item = null;

            int firstOffset = data.GetOffset("First");
            int lastOffset = data.GetOffset("Last");
            int hireOffset = data.GetOffset("HireDate");
            int enrollOffset = data.GetOffset("Enrollment");

            //Lets do some null checking on these 
            DBParam first = DBParam.ParamWithDelegate(DbType.String, delegate
            {
                return item[firstOffset];
            });

            DBParam second = DBParam.ParamWithDelegate(DbType.String, delegate
            {
                return item[lastOffset];
            });

            DBParam hire = DBParam.ParamWithDelegate(DbType.DateTime2, delegate
            {
                if (item.IsNull(hireOffset))
                    return DBNull.Value;
                else
                    return DateTime.Parse(item[hireOffset]);
            });

            DBParam enroll = DBParam.ParamWithDelegate(DbType.DateTime2, delegate
            {
                if (item.IsNull(enrollOffset))
                    return DBNull.Value;
                else
                    return DateTime.Parse(item[enrollOffset]);
            });

            DBQuery insert = DBQuery.InsertInto(Schools.Schema, Schools.Person.Table)
                                    .Fields(Schools.Person.First.Name,
                                            Schools.Person.Last.Name,
                                            Schools.Person.HireDate.Name,
                                            Schools.Person.EnrollmentDate.Name)
                                    .Values(first, second, hire, enroll);

            for (int i = 0; i < all.Length; i++)
            {
                item = all[i];
                //Do the insert and check that 1 was inserted
                Assert.AreEqual(1, db.ExecuteNonQuery(insert));
            }
        }

        #endregion

        #region public void SQLite_07_InsertInstructors()

        /// <summary>
        /// Looks up the IDs of the People with a string concatenation and inserts the id 
        /// along with the course into the instructors table.
        /// This handles fake rows in the data for both unknown course ID's and unknown people
        /// </summary>
        [TestMethod()]
        public void SQLite_07_InsertInstructors()
        {
            DBDatabase db = this.Database;

            CSVData instructors = CSVData.ParseString(SampleData.Instructors, true);
            CSVItem[] all = instructors.Items;
            CSVItem item = null;

            int courseOffset = instructors.GetOffset("Course");
            int personOffset = instructors.GetOffset("Instructor");
            int fakeOffset = instructors.GetOffset("Fake"); // a flag column for fake entries.

            string firstCol = Schools.Person.First.Name;
            string lastCol = Schools.Person.Last.Name;
            string pidCol = Schools.Person.Id.Name;

            //Paramter onto the course id of the current item.
            DBParam cid = DBParam.ParamWithDelegate("courseid", delegate { return item[courseOffset]; });

            //Parameter onto the full name of the person
            DBParam pname = DBParam.ParamWithDelegate("name", delegate { return item[personOffset]; });


            //We run an insert as a sub select. We don't know the autonumber Id of the 
            //person from the csv file, but we do know the full name.
            //The data is stored as FirstName and LastName so we can run a concatentation
            //in the where clause to compare it to the provided name
            // WHERE ([FirstName] + " " + [LastName] LIKE @name)
            
            DBQuery insert = DBQuery.InsertInto(Schools.Schema, Schools.Instructor.Table)
                                    .Fields(Schools.Instructor.CourseID.Name, Schools.Instructor.PersonID.Name)
                                    .Select(
                                        DBQuery.Select(cid, DBField.Field(pidCol))
                                               .From(Schools.Schema, Schools.Person.Table)
                                               .Where(DBFunction.Concat(DBField.Field(firstCol),DBConst.String(" "), DBField.Field(lastCol)), Compare.Like, pname)
                                               );

            for (int i = 0; i < all.Length; i++)
            {
                item = all[i];
                bool isfake = item[fakeOffset] == "True";

                int inserted = 0;
                try
                {
                    TestContext.WriteLine("Inserting {0}, {1} ", item[courseOffset], item[personOffset]);
                    inserted = db.ExecuteNonQuery(insert);

                    if (isfake)
                        Assert.AreEqual(0, inserted);
                    else
                        Assert.AreEqual(1, inserted);
                }
                catch (Exception ex)
                {
                    //If we are a fake row then this is OK, otherwise rethrow
                    if (!isfake)
                        throw;
                }
                //Validate that one was inserted as we don't allow invalid names
                
            }
        }

        #endregion


        //
        // indexes
        //

        #region public void SQLite_08_CreateIndexes()

        private const string DeptNameIndex = "DSQL_idx_DEPARTMENT_name";
        private const string CourseTitleIndex = "DSQL_idx_COURSE_title";

        /// <summary>
        /// Creates indexes on the Departments and courses tables
        /// </summary>
        [TestMethod()]
        public void SQLite_08_CreateIndexes()
        {
            DBDatabase db = this.Database;

            //Create unique index on Department name if it doesn't exist
            DBQuery create = DBQuery.Create.Index(DeptNameIndex).Unique().NonClustered().On(Schools.Schema, Schools.Departments.Table)
                                .Columns(Schools.Departments.Name.Name);
                                //SQLClient implementation does not support the IfNotExisits
            db.ExecuteNonQuery(create);
            TestContext.WriteLine("Successfully executed the create index");


            create = DBQuery.Create.Index(CourseTitleIndex).Unique().NonClustered()
                                    .On(Schools.Courses.Table)
                                    .Columns(Schools.Courses.Title.Name);
            db.ExecuteNonQuery(create);

            //Re-run the create WITHOUT the if not exists.
            //Should throw an error.
            bool failed;
            try
            {
                db.ExecuteNonQuery(create);
                failed = false;
            }
            catch (Exception ex)
            {
                failed = true;
                TestContext.WriteLine("Successfully caught the duplicate exception :{0}", ex);
            }

            if (!failed)
                throw new InvalidOperationException("The create should have thrown an exception when trying to create again");

        }

        #endregion

        //
        // update statements
        //

        #region public void SQLite_09_UpdateCourseAdministrators()

        /// <summary>
        /// Runs a number of statements against the department administrators
        /// to look them up in the people table and udate the value in the Departments table
        /// </summary>
        [TestMethod()]
        public void SQLite_09_UpdateCourseAdministrators()
        {
            DBDatabase db = this.Database;

            CSVData depts = CSVData.ParseString(SampleData.Departments, true);
            int deptnameOffset = depts.GetOffset("Name");
            int deptadminOffset=  depts.GetOffset("Admin");

            //Populate dictionaries to look up id's against names
            Dictionary<string, string> depts2admins =new Dictionary<string,string>();
            Dictionary<string, int> deptIds = new Dictionary<string, int>();
            Dictionary<string, int> adminIds = new Dictionary<string, int>();

            foreach (CSVItem item in depts.Items)
            {
                string dept = item[deptnameOffset];
                string admin = item[deptadminOffset];
                depts2admins.Add(dept,admin);
                deptIds.Add(dept, 0);
                adminIds.Add(admin, 0);
            }

            DBQuery selectdepts = DBQuery.Select().Fields(Schools.Departments.Name.Name, Schools.Departments.Id.Name)
                                                .From(Schools.Schema, Schools.Departments.Table)
                                                .WhereIn(Schools.Departments.Name.Name, GetConsts(depts2admins.Keys));

            DBQuery selectadmins = DBQuery.Select()
                                          .Field(Schools.Person.Id.Name)
                                          .Field(DBFunction.Concat(DBField.Field(Schools.Person.First.Name), DBConst.String(" "), DBField.Field(Schools.Person.Last.Name))).As("fullname")
                                          .From(Schools.Person.Table);
            
            //Fill the department ids based on the name
            db.ExecuteRead(selectdepts, reader =>
                {
                    while (reader.Read())
                    {
                        string deptname = reader.GetString(0);
                        if (deptIds.ContainsKey(deptname))
                            deptIds[deptname] = Convert.ToInt32(reader[1]);
                    }
                });

            //Fill the admin ids based on First Last
            db.ExecuteRead(selectadmins, reader =>
                {
                    while (reader.Read())
                    {
                        string adminfull = reader.GetString(1);
                        int id = Convert.ToInt32(reader[0]);

                        if (adminIds.ContainsKey(adminfull))
                            adminIds[adminfull] = id;
                    }
                });
            
            DBParam Padminid = DBParam.Param("adminid",DbType.Int32);
            DBParam Pdeptid = DBParam.Param("deptid",DbType.Int32);

            DBQuery update = DBQuery.Update(Schools.Departments.Table)
                                    .Set(Schools.Departments.Administrator.Name, Padminid)
                                    .WhereField(Schools.Departments.Id.Name, Compare.Equals, Pdeptid);

            //look up a person id and admin id based on department 
            //And execute the update if we have a match
            foreach (string deptname in depts2admins.Keys)
            {
                string adminame = depts2admins[deptname];
                int did = deptIds[deptname];
                int aid = adminIds[adminame];

                if (did > 0 && aid > 0)
                {
                    Padminid.Value = aid;
                    Pdeptid.Value = did;
                    db.ExecuteNonQuery(update);
                }
            }


            //Validation - select all the departments still with a null administrator
            DBQuery selectNulls = DBQuery.SelectFields(Schools.Departments.Id.Name, Schools.Departments.Name.Name)
                                         .From(Schools.Schema, Schools.Departments.Table)
                                         .WhereField(Schools.Departments.Administrator.Name, Compare.Is, DBConst.Null());
            
            List<string> unmatched = new List<string>();

            db.ExecuteRead(selectNulls, reader =>
            {
                while (reader.Read())
                {
                    unmatched.Add(reader.GetString(1));// Department Name.
                }
            });

            if (unmatched.Count > 0)
                TestContext.WriteLine("The following departments did not have matching Administrators '{0}'", string.Join(", ", unmatched.ToArray()));
            else
                TestContext.WriteLine("All departments had a matching Administrator");
        }

        private DBConst[] GetConsts(IEnumerable<string> values)
        {
            List<DBConst> all = new List<DBConst>();
            foreach (string value in values)
            {
                all.Add(DBConst.String(value));
            }
            return all.ToArray();
        }

        #endregion

        //
        // create views and stored procedures
        //

        #region public void SQLite_10_CreateCourseViews()

        private const string OnlyOnlineCoursesView = "DSQL_ONLY_ONLINE_COURSES";
        private const string OnlyOnSiteCoursesView = "DSQL_ONLY_ONSITE_COURSES";

        /// <summary>
        /// Creates 2 views for only online courses and only on-site courses and checks that they
        /// return the correct data. Make sure the account you are using has these permissions.
        /// </summary>
        [TestMethod()]
        public void SQLite_10_CreateCourseViews()
        {
            DBDatabase db = this.Database;

            DBQuery view = DBQuery.Create.View(Schools.Schema, OnlyOnlineCoursesView).As(
                DBQuery.Select()
                            .Field("C", Schools.Courses.Id.Name)
                            .Field("C", Schools.Courses.Title.Name)
                            .Field("C", Schools.Courses.Credits.Name)
                            .Field("C", Schools.Courses.Department.Name)
                            .Field("C", Schools.Courses.Description.Name)
                            .Field("OC", Schools.OnlineCourse.Url.Name)
                        .From(Schools.Schema, Schools.Courses.Table).As("C")
                            .InnerJoin(Schools.Schema, Schools.OnlineCourse.Table).As("OC")
                            .On("C", Schools.Courses.Id.Name, Compare.Equals, "OC", Schools.OnlineCourse.Id.Name)
                            );

            db.ExecuteNonQuery(view);
            TestContext.WriteLine("The view '" + OnlyOnlineCoursesView + "' was created into the schema");


            //Validate that the view exists and the number of rows returned
            //is the same for the OnlineCourses table and the new view.

            DBQuery selectview = DBQuery.SelectCount().From(Schools.Schema, OnlyOnlineCoursesView);
            int viewcount = Convert.ToInt32(db.ExecuteScalar(selectview));

            DBQuery selecttble = DBQuery.SelectCount().From(Schools.Schema, Schools.OnlineCourse.Table);
            int tblcount = Convert.ToInt32(db.ExecuteScalar(selecttble));

            Assert.AreEqual(tblcount, viewcount);
            TestContext.WriteLine("Validated that {0} rows are in the OnlineCourses table and {0} rows were returned from the OnlyOnlineCourses view", tblcount, viewcount);

            view = DBQuery.Create.View(Schools.Schema, OnlyOnSiteCoursesView).As(
                DBQuery.Select()
                            .Field("C", Schools.Courses.Id.Name)
                            .Field("C", Schools.Courses.Title.Name)
                            .Field("C", Schools.Courses.Credits.Name)
                            .Field("C", Schools.Courses.Department.Name)
                            .Field("C", Schools.Courses.Description.Name)
                            .Field("OC", Schools.OnsiteCourse.Location.Name)
                            .Field("OC", Schools.OnsiteCourse.Days.Name)
                            .Field("OC", Schools.OnsiteCourse.Time.Name)
                        .From(Schools.Schema, Schools.Courses.Table).As("C")
                            .InnerJoin(Schools.Schema, Schools.OnsiteCourse.Table).As("OC")
                            .On("C", Schools.Courses.Id.Name, Compare.Equals, "OC", Schools.OnsiteCourse.Id.Name)
                            );
            db.ExecuteNonQuery(view);
            TestContext.WriteLine("The view '" + OnlyOnSiteCoursesView + "' was created into the schema");


            //Validate that the view exists and the number of rows returned
            //is the same for the OnSiteCourses table and the new view.

            selectview = DBQuery.SelectCount().From(Schools.Schema, OnlyOnSiteCoursesView);
            viewcount = Convert.ToInt32(db.ExecuteScalar(selectview));

            selecttble = DBQuery.SelectCount().From(Schools.Schema, Schools.OnsiteCourse.Table);
            tblcount = Convert.ToInt32(db.ExecuteScalar(selecttble));

            Assert.AreEqual(tblcount, viewcount);
            TestContext.WriteLine("Validated that {0} rows are in the OnSiteCourses table and {0} rows were returned from the OnlyOnSiteCourses view", tblcount, viewcount);


        }

        #endregion

        #region public void SQLite_11_CreateOnsiteCourseSProc()

        public const string InsertOnSiteCourseSproc = "DSQL_sp_INSERT_ONSITE_COURSE";

        /// <summary>
        /// Creates a stored procedure for insert an on-site course
        /// </summary>
        [TestMethod()]
        public void SQLite_11_CreateOnsiteCourseSProc()
        {
            //STORED PROCEDURES NOT SUPPORTED
        }

        #endregion

        #region public void SQLite_12_CreateOnlineCourseSproc()

        public const string InsertOnlineCourseSproc = "DSQL_sp_INSERT_ONLINE_COURSE";

        /// <summary>
        /// Creates a stored procedure that inserts a new row in the Courses
        /// table and the Online courses table.
        /// </summary>
        [TestMethod()]
        public void SQLite_12_CreateOnlineCourseSproc()
        {
            //STORED PROCEDURES NOT SUPPORTED
        }

        #endregion

        //
        // a few CRUD operations on the schema
        //

        #region public void SQLite_13_SelectDepartmentCourses()

        /// <summary>
        /// Selects all the courses in a department including the 
        /// </summary>
        [TestMethod()]
        public void SQLite_13_SelectDepartmentCourses()
        {
            DBParam deptname = DBParam.Param("deptname", DbType.String, Schools.Departments.Name.Length);

            
            DBQuery sel = DBQuery.Select()
                                     .Field("D", Schools.Departments.Name.Name).As("dept_name")
                                     .Field("DAD", Schools.Person.First.Name).As("admin_first")
                                     .Field("DAD", Schools.Person.Last.Name).As("admin_last")
                                     .Field("C", Schools.Courses.Id.Name).As("course_id")
                                     .Field("C", Schools.Courses.Title.Name).As("course_title")
                                 .From(Schools.Schema, Schools.Courses.Table).As("C")
                                     .InnerJoin(Schools.Schema, Schools.Departments.Table).As("D")
                                                .On("C", Schools.Courses.Department.Name, Compare.Equals, "D", Schools.Departments.Id.Name)
                                     .InnerJoin(Schools.Schema, Schools.Person.Table).As("DAD")
                                                .On("D", Schools.Departments.Administrator.Name, Compare.Equals, "DAD", Schools.Person.Id.Name)
                                 .WhereField("D", Schools.Departments.Name.Name, Compare.Equals, deptname);

            DBDatabase db = this.Database;

            deptname.Value = "IT";
            int count = 0;

            //Execute for the IT department and enumerate through the courses.
            db.ExecuteRead(sel, reader =>
            {
                while (reader.Read())
                {
                    //Just check - department names should match the parameter value
                    Assert.AreEqual(deptname.Value, reader.GetString(0));
                    TestContext.WriteLine("Course title in {0} dept : {1}",
                                           reader.GetString(0), reader.GetString(4));
                    count++;
                }
            });
            TestContext.WriteLine("Total number of courses in the '{0}' department is '{1}'", deptname.Value, count);
                
        }

        #endregion

        #region public void SQLite_14_DeleteAnOnsiteCourse()

        /// <summary>
        /// Deletes an onsite course that was created by the insert procedure.
        /// As the FK has a cascade delete this should also delete the ONSITE_COURSE related row
        /// </summary>
        [TestMethod()]
        public void SQLite_14_DeleteAnOnsiteCourse()
        {
            DBConst cid = DBConst.String("CPNN"); //This is the ID of the course that was previously created

            DBQuery del = DBQuery.DeleteFrom(Schools.Courses.Table)
                                 .WhereFieldEquals(Schools.Courses.Id.Name, cid);

            DBDatabase db = this.Database;

            //Get the original counts.
            DBQuery countCourses = DBQuery.SelectCount().From(Schools.Schema, Schools.Courses.Table);
            DBQuery countOnSite = DBQuery.SelectCount().From(Schools.Schema, Schools.OnsiteCourse.Table);
            
            int ccount = Convert.ToInt32(db.ExecuteScalar(countCourses));
            int oscount = Convert.ToInt32(db.ExecuteScalar(countOnSite));

            db.ExecuteNonQuery(del);

            int new_ccount = Convert.ToInt32(db.ExecuteScalar(countCourses));
            int new_oscount = Convert.ToInt32(db.ExecuteScalar(countOnSite));

            Assert.AreEqual(ccount - 1, new_ccount,"COURSE was not deleted");
            Assert.AreEqual(oscount - 1, new_oscount, "ONSITE_COURSE was not deleted automatically");
            TestContext.WriteLine("Deleted one row from courses and the corresponding row was automatically deleted from the related table");
            TestContext.WriteLine("New course count is {0}", ccount);
        }

        #endregion

        //
        // schema interrogation
        //


        [TestMethod()]
        public void SQLite_15_ValidateTableSchema()
        {
            DBDatabase db = this.Database;


            //List of all the schema.table names in upper case.
            List<string> all = new List<string>(new string[] {
                Schools.Schema.ToUpper() + "." + Schools.Courses.Table.ToUpper(), 
                Schools.Schema.ToUpper() + "." + Schools.Departments.Table.ToUpper(), 
                Schools.Schema.ToUpper() + "." + Schools.Instructor.Table.ToUpper(),
                Schools.Schema.ToUpper() + "." + Schools.OnlineCourse.Table.ToUpper(), 
                Schools.Schema.ToUpper() + "." + Schools.OnsiteCourse.Table.ToUpper(), 
                Schools.Schema.ToUpper() + "." + Schools.Person.Table.ToUpper()});

            //Load all the tables from the database provider
            Schema.DBSchemaProvider provider = db.GetSchemaProvider();
            IEnumerable<Schema.DBSchemaItemRef> tables = provider.GetAllTables();

            //Go through each of the tables returned and 
            //based on their schema.name remove them from 
            //the collection if they are in there.
            //At the end we should have an empty collection

            foreach (Schema.DBSchemaItemRef tableref in tables)
            {
                string name = tableref.Name.ToUpper();
                string owner = tableref.Schema.ToUpper();
                string full = owner + "." + name;

                if (all.Contains(full))
                    all.Remove(full);
            }

            Assert.AreEqual(0, all.Count, "Not all the tables were returned from the schema provider");
            TestContext.WriteLine("All created tables we found and returned from the SchemaProvider");

            Schema.DBSchemaTable table = provider.GetTable("", Schools.Schema, Schools.Courses.Table);
            this.ValidateTableColumns(table, Schools.Courses.Id, Schools.Courses.Credits, Schools.Courses.Department, Schools.Courses.Description, Schools.Courses.Title);

            provider.GetAllIndexs();
        }

        private void ValidateTableColumns(Schema.DBSchemaTable table, params DBColumn[] columns)
        {
            foreach (DBColumn col in columns)
            {
                Assert.IsTrue(table.Columns.Contains(col.Name), "The table does not contain the column '{0}'", col.Name);
                Schema.DBSchemaTableColumn schemaCol = table.Columns[col.Name];
                Assert.IsNotNull(schemaCol);

                //No real comparison of datatypes available
                //Assert.AreEqual(col.Type, schemaCol.DbType);

                if ((col.Flags & DBColumnFlags.PrimaryKey) > 0)
                    Assert.IsTrue(schemaCol.PrimaryKey);

                
            }
        }


        [TestMethod()]
        public void SQLite_16_ValidateViewSchema()
        {
            DBDatabase db = this.Database;
            Schema.DBSchemaProvider provider = db.GetSchemaProvider();


            IEnumerable<Schema.DBSchemaItemRef> views = provider.GetAllViews();

            List<string> known = new List<string>(new string[] { OnlyOnlineCoursesView, OnlyOnSiteCoursesView });
            foreach (Schema.DBSchemaItemRef viewref in views)
            {
                bool isknown = false;
                if (string.Equals(viewref.Schema, Schools.Schema, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(viewref.Name, OnlyOnSiteCoursesView, StringComparison.OrdinalIgnoreCase))
                {
                    known.Remove(OnlyOnSiteCoursesView);
                    isknown = true;
                }

                else if (string.Equals(viewref.Schema, Schools.Schema, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(viewref.Name, OnlyOnlineCoursesView, StringComparison.OrdinalIgnoreCase))
                {
                    known.Remove(OnlyOnlineCoursesView);
                    isknown = true;
                }

                if (isknown)
                {
                    Schema.DBSchemaView fullview = provider.GetView(viewref);
                    Assert.AreNotEqual(fullview.Columns.Count, 0);
                }
            }

            Assert.IsTrue(known.Count == 0, "Not all the known views were returned");

        }

        [TestMethod()]
        public void SQLite_17_ValidateIndexSchema()
        {
            DBDatabase db = this.Database;
            Schema.DBSchemaProvider provider = db.GetSchemaProvider();


            IEnumerable<Schema.DBSchemaItemRef> indexes = provider.GetAllIndexs();

            List<string> known = new List<string>(new string[] { CourseTitleIndex.ToUpper(), DeptNameIndex.ToUpper() });

            foreach (Schema.DBSchemaItemRef item in indexes)
            {
                if (known.IndexOf(item.Name.ToUpper()) >= 0 && string.Equals(item.Schema, Schools.Schema, StringComparison.OrdinalIgnoreCase))
                {
                    known.Remove(item.Name.ToUpper());

                    Schema.DBSchemaIndex idx = provider.GetIndex(item);
                    Assert.IsNotNull(idx, "No index was returned for the refrerence {0}", item);
                    Assert.AreNotEqual(idx.Columns.Count, 0);
                }
            }

            Assert.AreEqual(0, known.Count, "Not all known indexes were removed");

        }

        [TestMethod()]
        public void SQLite_18_ValidateSprocSchema()
        {
            //NOT SUPPORTED
        }

        // 
        // cleanup - Drop any schema elements that were created
        //

        #region public void SQLite_20_TearDownSchema()

        /// <summary>
        /// Clean up all the created items. If you create anything then the corresponding drop should be in here too.
        /// </summary>
        [TestMethod()]
        public void SQLite_20_TearDownSchema()
        {
            TestContext.WriteLine("Cleaning up database objects");

            DBQuery[] all = new DBQuery[] {

                
                DBQuery.Drop.View(Schools.Schema,OnlyOnSiteCoursesView).IfExists(),
                DBQuery.Drop.View(Schools.Schema,OnlyOnlineCoursesView).IfExists(),

                //DBQuery.Drop.Index(Schools.Schema,"idx_DeptName").IfExists(),     //uses the INFORMATION_SCHEMA.INDEXES view
                //DBQuery.Drop.Index(Schools.Schema,"idx_CourseTitle").IfExists(),  //if your version supports then execute the drop.
                                                                                    //Should be dropped with the tables anyway

                DBQuery.Drop.Table(Schools.Schema,Schools.Instructor.Table).IfExists(),
                DBQuery.Drop.Table(Schools.Schema,Schools.Person.Table).IfExists(),
                DBQuery.Drop.Table(Schools.Schema,Schools.OnlineCourse.Table).IfExists(),
                DBQuery.Drop.Table(Schools.Schema,Schools.OnsiteCourse.Table).IfExists(),
                DBQuery.Drop.Table(Schools.Schema,Schools.Courses.Table).IfExists(),
                DBQuery.Drop.Table(Schools.Schema,Schools.Departments.Table).IfExists()
            };

            DBDatabase db = this.Database;
            Exception failure = null;
            int failcount = 0;

            foreach (DBQuery q in all)
            {
                try
                {
                    db.ExecuteNonQuery(q);
                }
                catch (Exception ex)
                {
                    if (null == failure)
                        failure = ex;
                    failcount++;
                    TestContext.WriteLine("Clean up failed for '{0}'\r\n{1}", q.ToSQLString(db), ex);
                }
            }

            if (failcount > 0)
            {
                throw new Exception("There were '" + failcount.ToString() + 
                                    "' failures executing the drop schema tests. The first of which was '" 
                                    + failure.Message + "'", failure);
            }
        }

        #endregion

    }
}
