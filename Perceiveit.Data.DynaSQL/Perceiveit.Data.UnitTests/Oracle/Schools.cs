using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.UnitTests.Oracle
{
    /// <summary>
    /// The full Schema of the School for the SQL Server database
    /// </summary>
    public class Schools
    {
        /// <summary>
        /// The connection string to the SQL server database
        /// </summary>
        public const string DbConnection = "DATA SOURCE=127.0.0.1;PERSIST SECURITY INFO=True;USER ID=DBUSER;PASSWORD=Test";

        /// <summary>
        /// The provider name for SQL server
        /// </summary>
        public const string DbProvider = "System.Data.OracleClient";

        /// <summary>
        /// If this is set then the schema needs to be created
        /// </summary>
        public const string Schema = "DBUSER";

        public class Departments
        {
            public const string Table = "DSQL_DEPARTMENT";
            public const string SequenceName = "DSQL_DEPT_SEQUENCE";
            public static readonly DBColumn Id = DBColumn.Column("DepartmentID", System.Data.DbType.Int32, DBColumnFlags.PrimaryKey);
            public static readonly DBColumn Name = DBColumn.Column("Name", System.Data.DbType.String, 100);
            public static readonly DBColumn DateCreated = DBColumn.Column("Created", System.Data.DbType.DateTime2).Default(DBFunction.GetDate());
            public static readonly DBColumn LastModified = DBColumn.Column("LastModified", System.Data.DbType.DateTime2).Default(DBFunction.GetDate());
            public static readonly DBColumn Budget = DBColumn.Column("Budget", System.Data.DbType.Decimal, DBColumnFlags.Nullable);
            public static readonly DBColumn Administrator = DBColumn.Column("Administrator", System.Data.DbType.Int32, DBColumnFlags.Nullable);
        }

        public class Courses
        {
            public const string Table = "DQSL_COURSES";
            public static readonly DBColumn Id = DBColumn.Column("CourseID", System.Data.DbType.AnsiStringFixedLength, 4, DBColumnFlags.PrimaryKey);
            public static readonly DBColumn Title = DBColumn.Column("Title", System.Data.DbType.String, 100);
            public static readonly DBColumn Credits = DBColumn.Column("Credits", System.Data.DbType.Single, 20).Default(DBConst.Const(System.Data.DbType.Single, 0.0F));
            public static readonly DBColumn Department = DBColumn.Column("DepartmentID", System.Data.DbType.Int64);
            public static readonly DBColumn Description = DBColumn.Column("Description", System.Data.DbType.String,1000,DBColumnFlags.Nullable);
        }

        public class OnsiteCourse
        {
            public const string Table = "DSQL_ONSITECOURSE";
            public static readonly DBColumn Id = DBColumn.Column("CourseID", System.Data.DbType.AnsiStringFixedLength, 4, DBColumnFlags.PrimaryKey);
            public static readonly DBColumn Location = DBColumn.Column("Location", System.Data.DbType.String, 200, DBColumnFlags.Nullable);
            public static readonly DBColumn Days = DBColumn.Column("Days", System.Data.DbType.Int32).Default(DBConst.Int32(0));
            public static readonly DBColumn Time = DBColumn.Column("Time", System.Data.DbType.Time);

        }

        public class OnlineCourse
        {
            public const string Table = "DSQL_ONLINECOURSE";
            public static readonly DBColumn Id = DBColumn.Column("CourseID", System.Data.DbType.AnsiStringFixedLength, 4, DBColumnFlags.PrimaryKey);
            public static readonly DBColumn Url = DBColumn.Column("Url", System.Data.DbType.String, 100);
        }

        public class Person
        {
            public const string Table = "DSQL_PERSON";
            public const string Sequence = "DSQL_PERS_SEQUENCE";
            public static readonly DBColumn Id = DBColumn.Column("PersonID", System.Data.DbType.Int32, DBColumnFlags.PrimaryKey);
            public static readonly DBColumn First = DBColumn.Column("FirstName", System.Data.DbType.String, 100);
            public static readonly DBColumn Last = DBColumn.Column("LastName", System.Data.DbType.String, 100);
            public static readonly DBColumn HireDate = DBColumn.Column("HireDate", System.Data.DbType.DateTime2, DBColumnFlags.Nullable);
            public static readonly DBColumn EnrollmentDate = DBColumn.Column("EnrollmentDate", System.Data.DbType.DateTime2, DBColumnFlags.Nullable);
        }

        public class Instructor
        {
            //Compound primary key so create this indepenently 

            public const string Table = "DSQL_COURSEINSTRUCTOR";
            public static readonly DBColumn CourseID = DBColumn.Column("CourseID", System.Data.DbType.AnsiStringFixedLength, 4);
            public static readonly DBColumn PersonID = DBColumn.Column("PersonID", System.Data.DbType.Int32);

        }
    }
}
